using System.IO;

namespace Exp2_WPF
{
    public class LL1Analysis
    {
        //用$表示空
        private static readonly List<char> VtPremade = new()
        {   '+', '-', '*', '/', '(',')',
            'a', 'b', 'c', 'd', 'e',
            'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z' };


        private bool initialized { get; set; }
        private List<string> grammarLines { get; set; }
        private char startWord { get; set; }
        private HashSet<char> vt { get; set; }
        private HashSet<char> vn { get; set; }
        private Dictionary<char, HashSet<char>> first { get; set; }
        private Dictionary<char, HashSet<char>> follow { get; set; }
        private Dictionary<(char, string), HashSet<char>> select { get; set; }
        private Dictionary<(char, char), string> analysisTable { get; set; }
        private Dictionary<char, List<string>> grammar { get; set; }
        
        private List<StepInfo> stepInfos { get; set; }

        public Dictionary<char, List<string>> PrehandledGrammar => grammar;
        public Dictionary<(char, char), string> AnalysisTable => analysisTable;
        
        public List<StepInfo> StepInfos => stepInfos;

        public LL1Analysis()
        {
            grammarLines = new();
            startWord = ' ';
            vt = new ();
            vn = new();
            first = new();
            follow = new();
            select = new ();
            analysisTable = new ();
            grammar = new ();
            stepInfos = new();
        }
        public bool Initialized => initialized;
        
        
        /// <summary>
        /// 从文件中读取文法表
        /// </summary>
        /// <param name="filePath">要读取的文件路径</param>
        public void ReadGrammarFromFile(string filePath)
        {
            grammarLines = File.ReadAllLines(filePath).ToList();
        }
        /// <summary>
        /// 打印Vt和Vn
        /// </summary>
        public void PrintVtAndVn()
        {
            if (!initialized)
            {
                Console.WriteLine("请先读入文法表并进行预处理");
                return;
            }
            Console.Write("Vt:");
            foreach (var x in vt)
            {
                Console.Write(x+",");
            }
            Console.WriteLine();
            Console.Write("Vn:");
            foreach (var x in vn)
            {
                Console.Write(x+",");
            }
            Console.WriteLine();

        }
        
        
        /// <summary>
        /// 将输入的文法分成两部分,同时返回终止符和非终止符 ,以及文法的开始符号
        /// 如P->aE|B 分成P,{aE,B}这两部分
        /// </summary>
        public void PreprocessGrammars()
        {
            if (grammarLines.Count == 0)
            {
                Console.WriteLine("文法的数量为0,请先输入文法!");
                return;
            }
            Dictionary<char, List<string>> spiltGrammar = new();
            int firstIndex = grammarLines[0].IndexOf("->");
            if (firstIndex == -1)
            {
                Console.WriteLine("文法格式错误,请修改");
                return;
            }
            startWord = grammarLines[0].Substring(0, firstIndex)[0];
            //文法的开始符号
            try
            {
                foreach (var line in grammarLines)
                {
                    int arrowIndex = line.IndexOf("->");
                    if (arrowIndex == -1)
                    {
                        Console.WriteLine("文法格式错误,请修改");
                        return;
                    }
                    char left = line.Substring(0, arrowIndex)[0];
                    //左部添加到非终结符集合中
                    vn.Add(left);
                    string right = line.Substring(arrowIndex + 2);
                    List<string> rightWords = right.Split('|').ToList();
                    foreach (var rightWord in rightWords)
                    {
                        foreach (var c in rightWord)
                        {
                            if (VtPremade.Contains(c))
                            {
                                vt.Add(c);
                            }
                            else if(c!='$')
                            {
                                vn.Add(c);
                            }
                        }
                    }
                    var existingWords = spiltGrammar.GetOrCreate(left);
                    existingWords.AddRange(rightWords);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("文法的格式不正确,请修改!");
            }

            //没有错误即初始化成功
            initialized = true;
            grammar = spiltGrammar;
        }
        
        /// <summary>
        /// 求非终结符c的first集合
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private HashSet<char> GetOneFirst(char c)
        {
            // 如果缓存中已经存在这个非终结符的FIRST集，直接返回
            if (first.ContainsKey(c))
            {
                Console.WriteLine($"First中已经有{c}");
                return first[c];
            }
            //初始化first集合
            HashSet<char> oneFirst = new();
            foreach (var item in grammar[c])
            {
                for (int i = 0; i < item.Length; i++)
                {
                    char symbol = item[i];
                    //首字符是终结符，直接加入first集合中
                    if (vt.Contains(symbol))
                    {
                        oneFirst.Add(symbol);
                        break;
                    }
                    //首字符是非终结符,求出首字符的first后加入first
                    if (vn.Contains(symbol))
                    {
                        Console.WriteLine($"首字符{symbol}是非终结符,求出首字符的first后加入first");
                        var nextFirst = GetOneFirst(symbol);
                        foreach (var symbolFirst in nextFirst)
                        {
                            if (symbolFirst != '$')
                            {
                                oneFirst.Add(symbolFirst);
                            }
                        }
                        //首字符的first不包含空，也就是无法推出空，不需要再向后推导，first集合已经求出
                        if (!nextFirst.Contains('$'))
                        {
                            break;
                        }
                    }
                    //首字符是$，加入first集合
                    else if (symbol == '$')
                    {
                        oneFirst.Add('$');
                        break;
                    }
                }
            }
            // 缓存计算结果
            first.Add(c,oneFirst);
            return oneFirst;
        }
        
        /// <summary>
        /// 求所有非终结符的first集合
        /// </summary>
        public void GetFirstSet()
        {
            if (vn.Count == 0)
            {
                Console.WriteLine("非终结符的数量为0,不能求First集合");
                return;
            }
            foreach (var nonTerminal in vn)
            {
                GetOneFirst(nonTerminal);
            }
        }
        
        /// <summary>
        /// 打印First集合
        /// </summary>
        public void PrintFirstSet()
        {
            foreach (var pair in first)
            {
                Console.Write("First(" + pair.Key + ") = { ");
                var elements = pair.Value.ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    Console.Write(elements[i]);
                    if (i < elements.Length - 1)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine(" }");
            }
        }

        /// <summary>
        /// 求非终结符b的Follow集合
        /// </summary>
        /// <param name="b"></param>
        /// <param name="processing">正在处理的集合,防止无限递归</param>
        /// <returns></returns>

        private HashSet<char> getOneFollow(char b, HashSet<char> processing)
        {
            // 如果已经找到b的follow集合，不再找
            if (follow.ContainsKey(b))
            {
                return follow[b];
            }

            // 如果正在计算这个符号，直接返回空集合，防止无限递归
            if (processing.Contains(b))
            {
                return new HashSet<char>();
            }
            // 标记当前符号正在计算
            processing.Add(b);
            var oneFollow = new HashSet<char>();
            // 如果是起始符号，则将空 '$' 加入 FOLLOW 集合
            if (b == startWord)
            {
                oneFollow.Add('$');
            }
            foreach (char key in grammar.Keys)
            {
                foreach (var production in grammar[key])
                {
                    for (int i = 0; i < production.Length; i++)
                    {
                        if (production[i] == b)
                        {
                            bool addFollowKey = true;

                            for (int j = i + 1; j < production.Length; j++)
                            {
                                char nextChar = production[j];
                                //下一个字符为终结符,也就是 A->Bb的情况 
                                //将这个非终结符加入follow(B)
                                if (vt.Contains(nextChar))
                                {
                                    oneFollow.Add(nextChar);
                                    addFollowKey = false;
                                    break;
                                }
                                //下一个字符数非终结符A->aBC的情况
                                if (vn.Contains(nextChar))
                                {
                                    //将first(C)加入follow(B) 注意排除$
                                    foreach (var c in first[nextChar])
                                    {
                                        if (c != '$')
                                        {
                                            oneFollow.Add(c);
                                        }
                                    }
                                    //如果nextChar能推出空,那么将follow(A)加入到follow(B)
                                    if (!first[nextChar].Contains('$'))
                                    {
                                        addFollowKey = false;
                                        break;
                                    }
                                }
                            }
                            if (addFollowKey)
                            {
                                oneFollow.UnionWith(getOneFollow(key,processing));
                            }
                        }
                    }
                }
            }

            // 缓存计算结果
            follow[b] = oneFollow;
            // 计算完成，移除标记
            processing.Remove(b);
            return oneFollow;
        }
        
        private HashSet<char> getOneFollowWrapper(char b)
        {
            // 初始调用时，传递一个新的processing集合
            return getOneFollow(b,  new HashSet<char>());
        }
        
        /// <summary>
        /// 求所有非终结符的Follow集合
        /// </summary>
        public void GetFollowSet()
        {
            if (vn.Count == 0)
            {
                Console.WriteLine("非终结符的数量为0,不能求Follow集合");
                return;
            }
            foreach (var c in vn)
            {
                getOneFollowWrapper(c);
            }
        }
        
        /// <summary>
        /// 打印Follow集合
        /// </summary>
        public void PrintFollowSet()
        {
            foreach (var pair in follow)
            {
                Console.Write("Follow(" + pair.Key + ") = { ");
                var elements = pair.Value.ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    Console.Write(elements[i]);
                    if (i < elements.Length - 1)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine(" }");
            }
        }
        
        /// <summary>
        /// 求Select集合
        /// </summary>
        private void GetSelectSet()
        {
            var selectSets = new Dictionary<(char, string), HashSet<char>>();
            foreach (var key in grammar.Keys)
            {
                foreach (var production in grammar[key])
                {
                    var selectSet = new HashSet<char>();
                    //A->X 如果X能推导出空,那么Select(A->X) = First(X) - null + Follow(A)
                    bool flag = true;
                   
                    //判断是否右部是否推导出空的逻辑， 每个部分都能推导出空才能推导出空
                    //如果右部为空串,那么能推导出空
                    if(production != "$")
                    {
                        for (int i = 0; i < production.Length; i++)
                        {
                            if (vt.Contains(production[i]))
                            {
                                //包含终结符，不能推导出空
                                flag = false;
                                break;
                            }
                            //右部每个字符必须都是非终结符而且其first集都能推导出空才能推导出空
                            if (!(first.ContainsKey(production[i]) && first[production[i]].Contains('$')))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    
                    //右部能推导出空  First(X) - null + Follow(A)
                    if (flag)
                    {
                        //求First(X)
                        //如果X的第一个字符是终结符
                        if (vt.Contains(production[0]))
                        {
                            selectSet.Add(production[0]);
                        }
                        //第一个字符是非终结符
                        //X不为空时 X为空first(X)同样为空，不需要加
                        else if (first.ContainsKey(production[0]))
                        {
                            selectSet.UnionWith(first[production[0]]);
                        }
                        //加上Follow(A)
                        selectSet.UnionWith(follow[key]);

                    }
                    //右部不能推导出空 Select(A->X) = First(X)。
                    else
                    {
                        //求First(X)
                        //如果X的第一个字符是终结符
                        if (vt.Contains(production[0]))
                        {
                            selectSet.Add(production[0]);
                        }
                        //第一个字符是非终结符
                        //X不为空时 X为空first(X)同样为空，不需要加
                        else if (first.ContainsKey(production[0]))
                        {
                            selectSet.UnionWith(first[production[0]]);
                        }
                    }
                    selectSets[(key, production)] = selectSet;
                }
            }
            select = selectSets;
        }

        
        /// <summary>
        /// 构建分析表
        /// </summary>
        public void CreateAnalysisTable()
        {
            
            var table = new Dictionary<(char, char), string>();
            GetSelectSet();
            var selectSets = select;

            foreach (var key in grammar.Keys)
            {
                foreach (var production in grammar[key])
                {
                    var selectSet = selectSets[(key, production)];
                    foreach (var terminal in selectSet)
                    {
                        table[(key, terminal)] = production;
                    }
                }
                //当一个文法能推出空时，说明其遇到终止符能通过推空来消去左部
                if (grammar[key].Contains("$"))
                {
                    table[(key, '#')] = "$";
                    table.Remove((key, '$'));
                }
            }
            analysisTable = table;
        }
        
        /// <summary>
        /// 打印分析表
        /// </summary>
        public void PrintAnalysisTable()
        {
            Console.WriteLine("LL(1) Parsing Table:");
            foreach (var entry in analysisTable)
            {
                Console.WriteLine($"M[{entry.Key.Item1}, {entry.Key.Item2}] = {entry.Key.Item1}->{entry.Value}");
            }
        }

        /// <summary>
        /// 使用分析表对输入的字符串进行语法分析
        /// </summary>
        /// <param name="input"></param>
        public string ParseInput(string input)
        {
            if (input[^1] != '#')
            {
                Console.WriteLine("要分析的字符串格式不正确！");
                return "ERROR: 要分析的字符串格式不正确！";
            }
            if (analysisTable.Count == 0)
            {
                Console.WriteLine("请先构建分析表!");
                return "ERROR: 请先构建分析表!";
            }
            //清除上次的StepInfos，重新构建
            stepInfos = new List<StepInfo>();
            Stack<char> stack = new Stack<char>();
            string left = input;
            int inputLength = input.Length;

            int stepCount = 0;
            
            Console.WriteLine("动作:初始化");
            stack.Push('#');
            stack.Push(startWord); // 将开始的左部放到栈中
            Console.WriteLine($"开始词:{startWord}");
            int index = 0;
            var stackInfo = new string(stack.Reverse().ToArray());
            Console.WriteLine("Parsing Process:");
            Console.WriteLine($"Stack: {stackInfo}, Input: {input[index..]}");
            
            stepInfos.Add(new StepInfo()
            {
                StepCount = 0,
                Action = "初始化",
                AnalysisStack = stackInfo,
                ProductionUsed = "",
                RemainingInput = input[index..]
            });
            stepCount++;

            while (stack.Count > 0)
            {
                StepInfo thisStep = new StepInfo();
                
                // 剩余输入串
                left = input.Substring(index);
                Console.WriteLine("剩余输入串: "+left);
                
                thisStep.StepCount = stepCount;

                thisStep.RemainingInput = left;
                
                char top = stack.Peek();
                
                // 成功匹配输入串
                if (top == '#' && input[index] == '#')
                {
                    Console.WriteLine("Success: 输入串匹配成功");
                    return "Success: 输入串匹配成功";
                }

                // 如果栈顶是终止符时尝试与输入的字符串匹配
                if (vt.Contains(top))
                {
                    if (top == input[index])
                    {
                        stack.Pop();
                        thisStep.Action += "GETNEXT(I)";
                        index++;
                        Console.WriteLine($"匹配成功: {top}");
                    }
                    else
                    {
                        Console.WriteLine($"ERROR: 匹配失败,栈顶元素 {top} 和输入 {input[index]} 不匹配");
                        return $"ERROR: 匹配失败,栈顶元素 {top} 和输入 {input[index]} 不匹配";
                    }
                }
                // 栈顶是非终止符时查分析表
                else
                {
                    Console.WriteLine($"查分析表 M({top}, {input[index]})");
                    if (analysisTable.ContainsKey((top, input[index])))
                    {
                        stack.Pop();
                        thisStep.Action += "POP";
                        string production = analysisTable[(top, input[index])];
                        Console.WriteLine($"Output: {top}->{production}");
                        thisStep.ProductionUsed = $" {top}->{production}";

                        if (production != "$")
                        {
                            string pushInfo = "";
                            //将产生式右部倒序入栈
                            for (int i = production.Length - 1; i >= 0; i--)
                            {
                                stack.Push(production[i]);
                                pushInfo += production[i];
                            }
                            thisStep.Action += $",PUSH({pushInfo})";
                        }
                    }
                    else
                    {
                        Console.WriteLine($"匹配 {input[index]} 时失败! 栈顶元素: {top}, 要匹配的元素 {input[index]}, 分析表中没有 M({top}, {input[index]})");
                        return $"ERROR: 匹配 {input[index]} 时失败! 栈顶元素: {top}, 要匹配的元素 {input[index]}, 分析表中没有 M({top}, {input[index]})";
                    }
                }
                // 输出栈中内容
                var anaStack = string.Join("", stack.Reverse());
                thisStep.AnalysisStack = anaStack;
                Console.WriteLine($"分析栈 {anaStack}");
                stepInfos.Add(thisStep);
                stepCount++;
            }
            Console.WriteLine("Unknown Error!");
            return "ERROR: Unknown Error!";
        }


        public void PrintStepInfo()
        {
            foreach (var stepInfo in stepInfos)
            {
                Console.WriteLine($"步骤:{stepInfo.StepCount}\t分析栈:{stepInfo.AnalysisStack}\t剩余输入串:{stepInfo.RemainingInput}\t所用产生式:{stepInfo.ProductionUsed}\t动作:{stepInfo.Action}");
            }
        }
    }
    
    
    
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
        }
    }
}






