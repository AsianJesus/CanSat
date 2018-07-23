class Flag(object):
    def __init__(self,defState = True):
        self.state = defState
    def getState(self):
        return self.state
    def setState(self,state:bool):
        self.state = state