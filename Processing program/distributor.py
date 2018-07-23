from threading import Thread
from queue import Queue
from cuiinterface import Flag
from processing import Packet
from dbproxy import DBCommand
from time import sleep
from session import Session

class Distributor(Thread):
    errorLog : Queue
    def __init__(self,input: Queue,dbOutput:Queue,fileIOF:Queue,fileIOS:Queue,opFlag: Flag, ses : Session):
        self.input = input
        self.dbout = dbOutput
        self.foutF = fileIOF
        self.foutS = fileIOS
        self.opFlag = opFlag
        self.exitFlag = False
        self.session = ses
        self.savedID,self.savedName = None,None
        super().__init__()
    def start(self):
        self.opFlag.setState(True)
        super().start()
    def run(self):
        while True:
            if self.exitFlag:
                break
            sleep(1/2)
            if not self.opFlag.getState():
                continue
            self.__checkSession()
            while not self.input.empty():
                try:
                    elem = self.input.get()
                    if isinstance(elem,Packet):
                        elem.flyID = self.session.getID()
                        self.__distrib(elem)
                except Exception as e:
                    Distributor.errorLog.put(e)
    def stop(self):
        self.opFlag.setState(False)
    def __distrib(self,element:Packet):
        self.__inputDB(element)
        self.__inputFiles(element)
    def __checkSession(self) ->bool:
        if self.savedID != self.session.getID() or self.savedName != self.session.getName():
            self.savedID = self.session.getID()
            self.savedName = self.session.getName()
            self.__insID()
    def __insID(self):
        self.dbout.put(DBCommand("ins_id",(self.session.getID(),self.session.getName(),)))
    def __inputDB(self,element:Packet):
        self.dbout.put(DBCommand("ins_data",element.getInfo()))
    def __inputFiles(self,element:Packet):
        self.foutF.put(",".join([str(x) for x in element.getInfo()]))
        self.foutS.put(";".join([str(x) for x in element.getInfo()]))
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
    def setErrorLog(errorLog:Queue):
        Distributor.errorLog = errorLog