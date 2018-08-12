from threading import Thread
from queue import Queue
from flag import Flag
from time import sleep

class FileIO(Thread):
    errorLog:Queue
    def __init__(self,fileName:str, input:Queue, opFlag: Flag):
        self.file = open(fileName,'a')
        self.input = input
        self.opFlag = opFlag
        self.exitFlag = False
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
            while not self.input.empty():
                self.__write(str(self.input.get(True,0.5)))
            self.__flush()
        self.file.close()
    def stop(self):
        self.opFlag.setState(False)
        self.__flush()
    def __read(self) -> str:
        return self.file.read(1024)
    def __write(self,msg:str):
        self.file.write("{}\n".format(msg))
    def __flush(self):
        self.file.flush()
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
    def setErrorLog(errorLog:Queue):
        FileIO.errorLog = errorLog