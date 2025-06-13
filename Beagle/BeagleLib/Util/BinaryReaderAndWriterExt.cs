using BeagleLib.VM;
using System.Runtime.CompilerServices;

namespace BeagleLib.Util;

public static class BinaryReaderAndWriterExt
{
    public static void Write(this BinaryWriter me, Command[] commands)
    {
        foreach (var command in commands)
        {
            me.Write(command);
        }
        me.Write(Command.EndOfScript);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this BinaryWriter me, Command command)
    {
        me.Write((byte)command.Operation);
        if (command.Operation == OpEnum.EndOfScript || command.CommandType == CommandTypeEnum.CommandOnly) return;

        float floatValue;
        if (command.CommandType == CommandTypeEnum.CommandPlusFloat) floatValue = command.ConstValue;
        else floatValue = command.Idx;

        me.Write(floatValue);
    }

    public static Span<Command> ReadCommands(this BinaryReader me, Span<Command> commandsBuffer)
    {
        var commandsBufferLength = 0;
        while (me.BaseStream.Position != me.BaseStream.Length)
        {
            var command = me.ReadCommand();
            if (command.Operation == OpEnum.EndOfScript) break;
            commandsBuffer[commandsBufferLength++] = command;
        }
        return commandsBuffer.Slice(0, commandsBufferLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Command ReadCommand(this BinaryReader me)
    {
        var operation = (OpEnum)me.ReadByte();
        if (operation == OpEnum.EndOfScript) return Command.EndOfScript;

        var commandType = operation.GetOperationProperties().CommandType;
        if (commandType == CommandTypeEnum.CommandOnly) return new Command(operation);

        var floatValue = me.ReadSingle();
        return new Command(operation, floatValue, true);
    }
}