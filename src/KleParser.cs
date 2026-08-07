using MelonLoader;
using UnityEngine;

namespace YT_Mod;

public class KeyData
{
    public string Key { get; set; } = "";
    public String Size { get; set; } = "";
}

public static class KleParser
{
    public static List<List<KeyData>> ParseKleJson(string jsonString)
    {
        var result = new List<List<KeyData>>();

        if (string.IsNullOrWhiteSpace(jsonString)) return result;

        jsonString = jsonString.Replace(" ", "").Replace("  ", "").Replace("\n", "");

        jsonString = jsonString.Substring(1, jsonString.Length - 3);
        
        var jsonStrings = jsonString.Split("],");

        MelonLogger.Msg($"Parsed {jsonStrings.Length} rows.");

        for (int y = 0; y < jsonStrings.Length; y++)
        {
            var line = jsonStrings[y].Remove(0, 1);
            var keysStr = line.Split(',');
            var condition = "";
            var conditionFlag = false;
            
            var currentRow = new List<KeyData>();
            
            for (int x = 0; x < keysStr.Length; x++)
            {
                // Replace neccecary
                var text = keysStr[x].Replace("\"", "").Replace(@"\\", @"\");
                // Start Condition tracking
                if (text[0] == '{')
                {
                    conditionFlag = true;
                    condition = "";
                }

                if (conditionFlag)
                {
                    condition += text;
                }
                
                // End Condition tracking
                if (text[text.Length - 1] == '}')
                {
                    conditionFlag = false;

                    x++;
                    
                    string codeCondition = "";
                    int i = 0;
                    
                    // Remove unnececary and necceccary changes
                    text = keysStr[x].Replace("\"", "").Replace(@"\\", @"\");
                    condition = condition.Replace("{", "").Replace("}", "");

                    // Parse Default Conditions
                    if (condition.Contains("x:"))
                    {
                        i = condition.IndexOf("x:")+2;
                        while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                        {
                            codeCondition += condition[i];
                            i++;
                        }

                        codeCondition += "|";

                    }
                    else
                    {
                        codeCondition += "0|";
                    }
                    
                    if (condition.Contains("y:"))
                    {
                        i = condition.IndexOf("y:")+2;
                        while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                        {
                            codeCondition += condition[i];
                            i++;
                        }

                        codeCondition += "|";

                    }
                    else
                    {
                        codeCondition += "0|";
                    }
                    
                    if (condition.Contains("w:"))
                    {
                        i = condition.IndexOf("w:")+2;
                        while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                        {
                            codeCondition += condition[i];
                            i++;
                        }

                        codeCondition += "|";

                    }
                    else
                    {
                        codeCondition += "1|";
                    }
                    
                    if (condition.Contains("h:"))
                    {
                        i = condition.IndexOf("h:")+2;
                        while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                        {
                            codeCondition += condition[i];
                            i++;
                        }
                    }
                    else
                    {
                        codeCondition += "1";
                    }

                    // Parse Extra Conditions
                    bool hasExtra = new List<string> { "x2:", "y2:", "w2:", "h2:" }.Any(item => condition.Contains(item));
                    if (hasExtra)
                    {
                        if (condition.Contains("x2:"))
                        {
                            codeCondition += "|";
                            
                            i = condition.IndexOf("x2:")+3;
                            while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                            {
                                codeCondition += condition[i];
                                i++;
                            }

                            codeCondition += "|";

                        }
                        else
                        {
                            codeCondition += "|0|";
                        }
                        
                        if (condition.Contains("y2:"))
                        {
                            i = condition.IndexOf("y2:")+3;
                            while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                            {
                                codeCondition += condition[i];
                                i++;
                            }

                            codeCondition += "|";

                        }
                        else
                        {
                            codeCondition += "0|";
                        }
                        
                        if (condition.Contains("w2:"))
                        {
                            i = condition.IndexOf("w2:")+3;
                            while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                            {
                                codeCondition += condition[i];
                                i++;
                            }

                            codeCondition += "|";

                        }
                        else
                        {
                            codeCondition += "1|";
                        }
                        
                        if (condition.Contains("h2:"))
                        {
                            i = condition.IndexOf("h2:")+3;
                            while (i < condition.Length && "0123456789.-".Contains(condition[i]))
                            {
                                codeCondition += condition[i];
                                i++;
                            }
                        }
                        else
                        {
                            codeCondition += "1";
                        }
                    }
                    
                    
                    // MelonLogger.Msg($"Key {x}: {text} | {codeCondition}");
                    
                    KeyData data = new KeyData();
                    data.Key = text;
                    data.Size = codeCondition;
                    currentRow.Add(data);
                    continue;
                }
                
                // NO Condition
                if (!conditionFlag)
                {
                    string codeCondition = "0|0|1|1";
                    // MelonLogger.Msg($"Key {x}: {text} | {codeCondition}");

                    KeyData data = new KeyData();
                    data.Key = text;
                    data.Size = codeCondition;
                    currentRow.Add(data);
                }
            }
            
            result.Add(currentRow);
        }

        return result;
    }
}