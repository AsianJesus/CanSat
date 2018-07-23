#pragma once
#include "dictionary.h"
#include "action.h"

enum Command {
	RELEASE,
	RESET,
	BEEP
};

class CommandList
{
private:
	Dictionary<Command, Action<void>> list;
	bool commandSet(Command) const;
public:
	CommandList();
	CommandList(Dictionary<Command, Action<void>>);
	CommandList(CommandList&);
	~CommandList();
	void Execute(Command);
	bool AllCommandsSet() const;
	void AddCommand(Command, Action<void>);
};
