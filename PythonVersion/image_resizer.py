import wx
import thread
import Queue
import traceback
import Image
import os
import shutil
import multiprocessing


def makedirs(d):
    if not os.path.isdir(d):
        try:
            os.makedirs(d)
        except Exception, e:
            pass
    
def resize_file(ff, sz, jo, dd, tp):
    if ff[-4:].lower() != '.jpg':
        if not jo and not tp:
            makedirs(dd)
            shutil.copyfileobj(
                open(ff, 'rb'),
                open(os.path.join(dd, os.path.basename(ff)), 'wb')
            )
            
    else:
        im = Image.open(ff)
        im.thumbnail((sz, sz), Image.ANTIALIAS)
        makedirs(dd)
        of = os.path.join(dd, os.path.basename(ff))
        if tp: of = u'%s_%spx%s' % (of[:-4], sz, of[-4:])
        im.save(of, "JPEG", quality=82)

def file_worker(idx):
    wd = g_worker_data[idx]
    
    while True:
        w = g_qfile.get()
        try:
            wd[0] = w[0]
            resize_file(*w)
            wd[0] = ''
            wd[1] += 1
        except Exception, e:
            print e
            print traceback.print_exc()
        g_qfile.task_done()

def waiting_worker(ctx):
    while True:
        w = g_qwaiting.get()
        g_qsrc.join()
        g_qfile.join()
        g_stat[1] += 1
        g_qwaiting.task_done()

def add_file(w):
    pa,sz,jo = w
    
    if os.path.isdir(pa):
        for v in os.walk(pa):
            for f in v[2]:
                wd = v[0]
                dd = u'%s_%spx%s' % (wd[:len(pa)], sz, wd[len(pa):])
                g_qfile.put( (os.path.join(wd, f), sz, jo, dd, False) )
    
    elif os.path.isfile(pa):
        g_qfile.put( (pa, sz, jo, os.path.dirname(pa), True) )
    
def src_worker(ctx):
    while True:
        w = g_qsrc.get()
        try:
            add_file(w)
        except Exception, e:
            print e
            print traceback.print_exc()
        g_qsrc.task_done()

class FileDropTarget(wx.FileDropTarget):
    def __init__(self, parent):
        wx.FileDropTarget.__init__(self)
        self.parent = parent
    
    def OnDropFiles(self, x, y, filenames):
        g_stat[0] += 1
        sz = int(self.parent.txt_size.GetValue())
        jo = self.parent.cbx_jpgonly.GetValue()
        for f in filenames:
            g_qsrc.put( (f, sz, jo) )
        g_qwaiting.put(None)
        
class Frame(wx.Frame):
    def __init__(self):
        wx.Frame.__init__(self, None, wx.ID_ANY, "Image Resizer", style=wx.DEFAULT_FRAME_STYLE)
        self.SetBackgroundColour('F2F2F2')
        self.SetFont(wx.Font(12, wx.FONTFAMILY_DEFAULT, wx.FONTSTYLE_NORMAL, wx.FONTWEIGHT_NORMAL))
        
        msz = wx.BoxSizer(wx.VERTICAL)
        self.SetSizer(msz)
        
        isz = wx.BoxSizer(wx.HORIZONTAL)
        msz.Add(isz, 0, wx.EXPAND|wx.ALL, 5)
        
        self.txt_size = wx.TextCtrl(self, -1, "2000", size=(120, -1))
        isz.Add(self.txt_size, 0, wx.EXPAND)
        
        self.cbx_jpgonly = wx.CheckBox(self, -1, "JPG Only")
        self.cbx_jpgonly.SetValue(True)
        isz.Add(self.cbx_jpgonly, 0, wx.EXPAND|wx.LEFT, 5)
        
        #
        panel = wx.Panel(self)
        msz.Add(panel, 1, (wx.EXPAND|wx.ALL)^wx.TOP, 5)
        panel.SetBackgroundColour('Yellow')
        panel.SetDropTarget(FileDropTarget(self))
        
        isz = wx.BoxSizer(wx.VERTICAL)
        panel.SetSizer(isz)
        
        st = wx.StaticText(panel, -1, "Drop Here", size=(300,-1), style=wx.ALIGN_CENTER|wx.ST_NO_AUTORESIZE)
        isz.Add(st, 0, wx.EXPAND)
        
        st = self.st_status = wx.StaticText(panel, -1, "", style=wx.ALIGN_CENTER|wx.ST_NO_AUTORESIZE)
        isz.Add(st, 0, wx.EXPAND|wx.TOP, 5)
        
        self.st_worker_status = [None, ] * g_max_worker
        for i in range(g_max_worker):
            st = self.st_worker_status[i] = wx.StaticText(panel, -1, "", style=wx.ALIGN_CENTER|wx.ST_NO_AUTORESIZE)
            isz.Add(st, 0, wx.EXPAND|wx.TOP, 5)
        
        isz.AddSpacer(5)
        msz.Fit(self)
        
        self.Bind(wx.EVT_TIMER, self.OnTimer)
        self.status_timer = wx.Timer(self)
        self.status_timer.Start(300)

    def OnTimer(self, evt):
        s = u'%s [%02d / %02d]' % (g_stat[1] == g_stat[0] and 'Done' or 'Running', g_stat[1], g_stat[0])
        self.st_status.SetLabel(s)
        for i in range(g_max_worker):
            st = g_worker_data[i]
            s = u'#%02d[%03d] - %s' % (i, st[1], os.path.basename(st[0]))
            self.st_worker_status[i].SetLabel(s)

########################################################

g_stat = [0, 0]

g_qsrc = Queue.Queue()
thread.start_new_thread(src_worker, (None,))

g_qwaiting = Queue.Queue()
thread.start_new_thread(waiting_worker, (None,))

g_qfile = Queue.Queue(256)
g_max_worker = max(1, multiprocessing.cpu_count() - 1)
g_worker_data = [None, ] * g_max_worker
for i in range(g_max_worker):
    g_worker_data[i] = ['', 0]
    thread.start_new_thread(file_worker, (i,))

g_app = wx.App(False)
g_frm = Frame()
g_frm.Show(True)
g_app.MainLoop()

