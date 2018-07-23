#include "stdafx.h"
#include "commandlist.h"

bool CommandList::commandSet(Command c) const
{
	return list.keyExists(c);
}

CommandList::CommandList() : list()
{

}

CommandList::CommandList(Dictionary<Command, Action<void>> dict) : list(dict)
{
}

CommandList::CommandList(CommandList & other)
{
	list = other.list;
}

CommandList::~CommandList()
{
}

void CommandList::Execute(Command c) 
{
	if (commandSet(c)) {
		list[c]->Execute();
	}
}

bool CommandList::AllCommandsSet() const
{
	return commandSet(Command::RELEASE) && commandSet(Command::RESET) && commandSet(Command::BEEP);
}

void CommandList::AddCommand(Command c, Action<void> act)
{
	list[c] = act;
}
