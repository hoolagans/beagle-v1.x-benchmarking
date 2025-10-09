using System.Globalization;
using BeagleLib.Agent;
using BeagleLib.VM;

namespace BeagleLib.MathStackLib;

public abstract record MathExpr
{
    #region Methods
    public static MathExpr FromJson(string json, string[] inputLabels)
    {
        var organism = Organism.CreateFromJson(json);
        return FromCommands(organism.Commands, inputLabels);
    }

    public static MathExpr FromCommands(Command[] commands, string[] inputLabels)
    {
        var queue = new Queue<ByteCode>();
        foreach (var command in commands)
        {
            switch (command.Operation)
            {
                case OpEnum.Add:
                    queue.Enqueue(new ByteCode.Add());
                    break;
                case OpEnum.Const:
                    queue.Enqueue(new ByteCode.Const(command.ConstValue));
                    break;
                case OpEnum.Div:
                    queue.Enqueue(new ByteCode.Div());
                    break;
                case OpEnum.Dup:
                    queue.Enqueue(new ByteCode.Dup());
                    break;
                case OpEnum.Del:
                    queue.Enqueue(new ByteCode.Del());
                    break;
                case OpEnum.Load:
                    queue.Enqueue(new ByteCode.Load(inputLabels[command.Idx]));
                    break;
                case OpEnum.Mul:
                    queue.Enqueue(new ByteCode.Mul());
                    break;
                case OpEnum.Sign:
                    queue.Enqueue(new ByteCode.Sign());
                    break;
                case OpEnum.Sqrt:
                    queue.Enqueue(new ByteCode.Sqrt());
                    break;
                case OpEnum.Cbrt:
                    queue.Enqueue(new ByteCode.Cbrt());
                    break;
                case OpEnum.Sub:
                    queue.Enqueue(new ByteCode.Sub());
                    break;
                case OpEnum.Swap:
                    queue.Enqueue(new ByteCode.Swap());
                    break;
                case OpEnum.Paste:
                    queue.Enqueue(new ByteCode.Paste(command.Idx));
                    break;
                case OpEnum.Square:
                    queue.Enqueue(new ByteCode.Square());
                    break;
                case OpEnum.Cube:
                    queue.Enqueue(new ByteCode.Cube());
                    break;
                case OpEnum.Ln:
                    queue.Enqueue(new ByteCode.Ln());
                    break;
                case OpEnum.Sin:
                    queue.Enqueue(new ByteCode.Sin());
                    break;
                case OpEnum.Pow:
                    queue.Enqueue(new ByteCode.Pow());
                    break;
                case OpEnum.Copy:
                    queue.Enqueue(new ByteCode.Copy(command.Idx));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return FromByteCode(queue);
    }

    public static MathExpr FromByteCode(Queue<ByteCode> instructions)
    {
        // So what we need to do here is rather simple
        // We create a stack for our math expressions,
        // And run through the virtual machine's instructions, in doing so we reduce 
        // Our math expressions down until the machine stops, and we have our final state
        // I believe the top value on the stack when finished is the output value?
        // We will go with this assumption for now.

        var registries = new Dictionary<int, MathExpr>();

        var stack = new List<MathExpr>();
        while (instructions.Any())
        {
            var instruction = instructions.Dequeue();
            switch (instruction)
            {
                // intentionally not getting clever here, just doing the operations plain for binary and unary is easier to read and maintain
                #region unary
                case ByteCode.Sign:
                    stack.Add(new Sign(stack.PopLast(1).First()));
                    break;
                case ByteCode.Sqrt:
                    stack.Add(new Sqrt(stack.PopLast(1).First()));
                    break;
                case ByteCode.Cbrt:
                    stack.Add(new Cbrt(stack.PopLast(1).First()));
                    break;
                case ByteCode.Square:
                    stack.Add(new Square(stack.PopLast(1).First()));
                    break;
                case ByteCode.Cube:
                    stack.Add(new Cube(stack.PopLast(1).First()));
                    break;
                case ByteCode.Ln:
                    stack.Add(new Ln(stack.PopLast(1).First()));
                    break;
                case ByteCode.Sin:
                    stack.Add(new Sin(stack.PopLast(1).First()));
                    break;
                case ByteCode.Abs:
                    stack.Add(new Abs(stack.PopLast(1).First()));
                    break;
                #endregion unary

                #region binary
                case ByteCode.Add:
                {
                    var rhs = stack.PopLast(1).First();
                    var lhs = stack.PopLast(1).First();
                    stack.Add(new Add(lhs, rhs));
                }
                break;

                case ByteCode.Sub:
                {
                    var rhs = stack.PopLast(1).First();
                    var lhs = stack.PopLast(1).First();
                    stack.Add(new Sub(lhs, rhs));
                }
                break;

                case ByteCode.Div:
                {
                    var rhs = stack.PopLast(1).First();
                    var lhs = stack.PopLast(1).First();
                    stack.Add(new Div(lhs, rhs));
                }
                break;

                case ByteCode.Mul:
                {
                    var rhs = stack.PopLast(1).First();
                    var lhs = stack.PopLast(1).First();
                    stack.Add(new Mul(lhs, rhs));
                }
                break;
                
                case ByteCode.Pow:
                {
                    var rhs = stack.PopLast(1).First();
                    var lhs = stack.PopLast(1).First();
                    stack.Add(new Pow(lhs, rhs));
                } 
                break;
                #endregion binary

                case ByteCode.Load(string name):
                    stack.Add(new Variable(name));
                    break;
                case ByteCode.Const(float value):
                    stack.Add(new Constant(value));
                    break;
                case ByteCode.Copy(int idx):
                    registries[idx] = stack.Last();
                    break;
                case ByteCode.Paste(int idx):
                    stack.Add(registries[idx]);
                    break;
                case ByteCode.Dup:
                    stack.Add(stack.Last());
                    break;
                case ByteCode.Swap:
                {
                    var temp1 = stack[^1];
                    var temp2 = stack[^2];
                    stack[^1] = temp2;
                    stack[^2] = temp1;
                }
                break;

                case ByteCode.Del:
                    stack.PopLast(1);
                    break;
                default:
                    throw new SystemException($"An unexpected member of the ByteCode record was encountered, this switch should be exhaustive: {instruction}");
            }
        }
        return stack.Last();
    }

    public string AsTraditionalString()
    {
        switch (this)
        {

            case Sign(MathExpr mathExpr):
                return $"(-1 * ({mathExpr.AsTraditionalString()}))";
            case Sqrt(MathExpr mathExpr):
                return $"({mathExpr.AsTraditionalString()})^(1/2)";
            case Cbrt(MathExpr mathExpr):
                return $"({mathExpr.AsTraditionalString()})^(1/3)";
            case Square(MathExpr mathExpr):
                return $"(({mathExpr.AsTraditionalString()})^2)";
            case Cube(MathExpr mathExpr):
                return $"(({mathExpr.AsTraditionalString()})^3)";
            case Ln(MathExpr mathExpr):
                return $"ln({mathExpr.AsTraditionalString()})";
            case Sin(MathExpr mathExpr):
                return $"sin({mathExpr.AsTraditionalString()})";
            case Abs(MathExpr mathExpr):
                return $"(|{mathExpr.AsTraditionalString()}|)";
            case Add(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsTraditionalString()}) + ({rhs.AsTraditionalString()}))";
            case Sub(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsTraditionalString()}) - ({rhs.AsTraditionalString()}))";
            case Div(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsTraditionalString()}) / ({rhs.AsTraditionalString()}))";
            case Mul(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsTraditionalString()}) * ({rhs.AsTraditionalString()}))";
            case Variable(string name):
                return name;
            case Constant(float value):
                return value.ToString(CultureInfo.InvariantCulture);
            default:
                throw new SystemException($"An unexpected member of the MathExpr record was encountered, this switch should be exhaustive: {this}");
        }
    }

    public string AsFunctionPlotString()
    {
        switch (this)
        {

            case Sign(MathExpr mathExpr):
                return $"(-1 * ({mathExpr.AsFunctionPlotString()}))";
            case Sqrt(MathExpr mathExpr):
                return $"nthRoot(({mathExpr.AsFunctionPlotString()}), 2)";
            case Cbrt(MathExpr mathExpr):
                return $"nthRoot(({mathExpr.AsFunctionPlotString()}), 3)";
            case Square(MathExpr mathExpr):
                return $"(pow(({mathExpr.AsFunctionPlotString()}), 2))";
            case Cube(MathExpr mathExpr):
                return $"(pow(({mathExpr.AsFunctionPlotString()}), 3))";
            case Ln(MathExpr mathExpr):
                return $"log(({mathExpr.AsFunctionPlotString()}))";
            case Sin(MathExpr mathExpr):
                return $"sin(({mathExpr.AsFunctionPlotString()}))";
            case Abs(MathExpr mathExpr):
                return $"(|({mathExpr.AsFunctionPlotString()})|)";
            case Add(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsFunctionPlotString()}) + ({rhs.AsFunctionPlotString()}))";
            case Sub(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsFunctionPlotString()}) - ({rhs.AsFunctionPlotString()}))";
            case Div(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsFunctionPlotString()}) / ({rhs.AsFunctionPlotString()}))";
            case Mul(MathExpr lhs, MathExpr rhs):
                return $"(({lhs.AsFunctionPlotString()}) * ({rhs.AsFunctionPlotString()}))";
            case Variable(string name):
                return name;
            case Constant(float value):
                return value.ToString(CultureInfo.InvariantCulture);
            default:
                throw new SystemException($"An unexpected member of the MathExpr record was encountered, this switch should be exhaustive: {this}");
        }
    }

    public string AsLatexString()
    {
        switch (this)
        {

            case Sign(MathExpr mathExpr):
                return $"-{{{mathExpr.AsLatexString()}}}";
            case Sqrt(MathExpr mathExpr):
                return $"\\sqrt{{{mathExpr.AsLatexString()}}}";
            case Cbrt(MathExpr mathExpr):
                return $"\\sqrt[3]{{{mathExpr.AsLatexString()}}}";
            case Square(MathExpr mathExpr):
                return $"{{{mathExpr.AsLatexString()}}}^2";
            case Cube(MathExpr mathExpr):
                return $"{{{mathExpr.AsLatexString()}}}^3";
            case Ln(MathExpr mathExpr):
                return $"\\ln({mathExpr.AsLatexString()})";
            case Sin(MathExpr mathExpr):
                return $"\\sin({mathExpr.AsLatexString()})";
            case Abs(MathExpr mathExpr):
                return $"|{mathExpr.AsLatexString()}|";
            case Add(MathExpr lhs, MathExpr rhs):
                return $"({lhs.AsLatexString()} + {rhs.AsLatexString()})";
            case Sub(MathExpr lhs, MathExpr rhs):
                return $"({lhs.AsLatexString()} - {rhs.AsLatexString()})";
            case Div(MathExpr lhs, MathExpr rhs):
                return $"(\\frac{{{lhs.AsLatexString()}}}{{{rhs.AsLatexString()}}})";
            case Mul(MathExpr lhs, MathExpr rhs):
                return $"({lhs.AsLatexString()} \\cdot {rhs.AsLatexString()})";
            case Pow(MathExpr lhs, MathExpr rhs):
                return $"({lhs.AsLatexString()} ^ {rhs.AsLatexString()})";
            case Variable(string name):
                return name;
            case Constant(float value):
                return value.ToString(CultureInfo.InvariantCulture);
            default:
                throw new SystemException($"An unexpected member of the MathExpr record was encountered, this switch should be exhaustive: {this}");
        }
    }
    #endregion

    #region Properties

    #region abstract
    public abstract record BinaryExpr : MathExpr
    {
        protected BinaryExpr(MathExpr lhs, MathExpr rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public MathExpr Lhs { get; protected set; }
        public MathExpr Rhs { get; protected set; }
    }
    public abstract record UnaryExpr : MathExpr
    {
        protected UnaryExpr(MathExpr inner)
        {
            Inner = inner;
        }
        public MathExpr Inner { get; protected set; }
    }
    #endregion

    #region unary
    public sealed record Sign(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Sqrt(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Cbrt(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Square(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Cube(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Ln(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Sin(MathExpr MathExpr) : UnaryExpr(MathExpr);
    public sealed record Abs(MathExpr MathExpr) : UnaryExpr(MathExpr);
    #endregion unary

    #region binary
    public sealed record Add(MathExpr Lhs, MathExpr Rhs) : BinaryExpr(Lhs, Rhs);
    public sealed record Sub(MathExpr Lhs, MathExpr Rhs) : BinaryExpr(Lhs, Rhs);
    public sealed record Div(MathExpr Lhs, MathExpr Rhs) : BinaryExpr(Lhs, Rhs);
    public sealed record Mul(MathExpr Lhs, MathExpr Rhs) : BinaryExpr(Lhs, Rhs);
    public sealed record Pow(MathExpr Lhs, MathExpr Rhs) : BinaryExpr(Lhs, Rhs);
    #endregion binary

    #region special
    public sealed record Variable(string Name) : MathExpr;
    public sealed record Constant(float Value) : MathExpr;
    #endregion

    #endregion
}