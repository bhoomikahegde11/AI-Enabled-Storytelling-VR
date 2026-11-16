"""Existing Portuguese page plus optional four-character Parler browser endpoints."""
from http.server import SimpleHTTPRequestHandler,ThreadingHTTPServer
from pathlib import Path
import os,json,socket,subprocess,time,urllib.request,webbrowser
from datetime import datetime,timezone
STUDIO=Path(__file__).resolve().parents[4]
backend=None;log=None;urlbase=None
def ready(port):
    try:
        with urllib.request.urlopen(f'http://127.0.0.1:{port}/indian-api/capabilities',timeout=1) as response:
            return json.load(response).get('version')=='indian-browser-consistency-v1'
    except Exception:return False
for port in [7860,*range(7862,7870)]:
    if ready(port):urlbase=f'http://127.0.0.1:{port}';break
if urlbase is None:
    for port in [7860,*range(7862,7870)]:
        sock=socket.socket()
        try:sock.bind(('127.0.0.1',port));sock.close();break
        except OSError:sock.close()
    else:raise SystemExit('No free Parler studio port. Close an older studio instance.')
    folder=STUDIO/'testing/indian_auditions/recording_browser';folder.mkdir(parents=True,exist_ok=True)
    logpath=folder/('server_'+datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')+'.log');log=logpath.open('x',encoding='utf-8')
    env=dict(os.environ,ANUBHAVA_STUDIO_NO_BROWSER='1',ANUBHAVA_STUDIO_PORT=str(port),HF_HUB_OFFLINE='1',TRANSFORMERS_OFFLINE='1')
    backend=subprocess.Popen([str(STUDIO/'.venv/Scripts/python.exe'),'-B','-u',str(STUDIO/'app.py')],cwd=STUDIO,env=env,stdout=log,stderr=subprocess.STDOUT,creationflags=getattr(subprocess,'CREATE_NO_WINDOW',0))
    for _ in range(80):
        if ready(port):urlbase=f'http://127.0.0.1:{port}';break
        if backend.poll() is not None:break
        time.sleep(.25)
    if urlbase is None:
        print('Indian backend unavailable. See',logpath,flush=True)
        if backend.poll() is None:backend.terminate()
        backend=None;urlbase=f'http://127.0.0.1:{port}'
class Handler(SimpleHTTPRequestHandler):
    def __init__(self,*a,**kw):super().__init__(*a,directory=str(STUDIO),**kw)
    def do_GET(self):
        if self.path=='/indian_backend.json':
            data=json.dumps(dict(url=urlbase)).encode();self.send_response(200);self.send_header('Content-Type','application/json');self.send_header('Content-Length',str(len(data)));self.end_headers();self.wfile.write(data)
        else:super().do_GET()
server=None
for port in range(8765,8775):
    try:server=ThreadingHTTPServer(('127.0.0.1',port),Handler);break
    except OSError:continue
if server is None:
    if backend:backend.terminate()
    raise SystemExit('Ports 8765–8774 busy; close an old launcher.')
url=f'http://127.0.0.1:{port}/production/batches/portuguese_ready_20261027/VOICE_VERIFICATION_STUDIO_v3.html'
print('Existing voice studio:',url,'Parler backend:',urlbase,flush=True)
print('No synthesis on startup. Ctrl+C closes this launcher and only the backend it started.',flush=True)
if os.environ.get('ANUBHAVA_LAUNCH_NO_BROWSER')!='1':webbrowser.open(url)
try:server.serve_forever()
except KeyboardInterrupt:pass
finally:
    server.server_close()
    if backend and backend.poll() is None:backend.terminate()
    if log:log.close()
