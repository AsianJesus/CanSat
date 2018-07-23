from threading import Thread
from queue import Queue
import time

class ExceptionHandler(Thread):
    def __init__(self, inputs: [], outputs:[]):
        self.inputs = inputs
        self.outputs = outputs
        self.isRunning = False
        self.exitFlag = False
        super().__init__()
    def start(self):
        self.isRunning = True
        super().start()
    def run(self)->None:
        while True:
            if self.exitFlag:
                break
            time.sleep(3/4)
            if not self.isRunning:
                continue
            if hasattr(self.inputs,"__iter__"):
                for input in self.inputs:
                    while not input.empty():
                        self.__handleException(input.get())
            else:
                while not self.inputs.empty():
                    self.__handleException(self.inputs.get())
    def stop(self)->None:
        self.isRunning = False
    def __output(self,msg:str)->None:
        if hasattr(self.outputs,"__iter__"):
            for output in self.outputs:
                output.put(msg)
        else:
            self.outputs.put(msg)
    def __handleException(self,exp : Exception)->None:
        recTime = time.localtime()
        line = "[{0}:{1}:{2}]{3}".format(recTime[3],recTime[4],recTime[5],exp)
        self.__output(line)
    def terminate(self):
        self.exitFlag = True
