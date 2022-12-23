using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DictionarySort
{
    public static class Extensions
    {
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        public static IList<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
    static class Sorting
    {
      public static void Sort(ref List<string> list, int type)
        {
            list = list.Where(x => x != ""&&x!=null).ToList();
           
            switch (type)
            {
                case (int)SortType.FirstWord:
                    SortByFirstWord(ref list);
                    break; 
                case (int)SortType.SecondWord:
                    SortBySecondWord(ref list);
                    break;

                default:
                    break;
            }
           
        }
    private static void SortByFirstWord(ref List<string> list) => list.Sort();

    private static void SortBySecondWord(ref List<string> list)
        { 
            string pattern = @".*\- *";
            List<string> buffer = new List<string>(list);
            
           
            for (int i = 0; i < buffer.Count; i++)
            {
                buffer[i] = Regex.Replace(buffer[i], pattern, String.Empty);
            }
            buffer.Sort();
            for (int i = 0; i < buffer.Count; i++)
            {
                for (int j = 0; j < buffer.Count; j++)
                {
                    if (buffer[i] == Regex.Replace(list[j], pattern, String.Empty))
                    {
                        list.Swap(i, j);
                    }
                }
            }
           

        }

    }
}
