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
        static public bool reverseSortFirst = false;
        static public bool reverseSortSecond = false;
        public static void sort(ref List<string> list, uint type)
        {
           
           
            switch (type)
            {
                case (int)SortType.FirstWord:
                    sortByFirstWord(ref list);
                    reverseSortFirst = !reverseSortFirst;
                    break; 
                case (int)SortType.SecondWord:
                    sortBySecondWord(ref list);
                    reverseSortSecond = !reverseSortSecond;
                    break;

                default:
                    break;
            }
           
        }
        private static void sortByFirstWord(ref List<string> list)
        {
            if (!reverseSortFirst)
                list?.Sort((a, b) => a.CompareTo(b));
            else
                list?.Sort((a, b) => b.CompareTo(a));

        }

    private static void sortBySecondWord(ref List<string> list)
        { 
            string pattern = @".*[-—]—* *";
            List<string> buffer = new List<string>();

            foreach (var item in list)
            {
                buffer.Add(Regex.Replace(item, pattern, String.Empty));
            }
          
            if (!reverseSortSecond)
                buffer?.Sort((a, b) => a.CompareTo(b));
            else
                buffer?.Sort((a, b) => b.CompareTo(a));
            for (int i = 0; i < buffer?.Count; i++)
            {
                for (int j = 0; j < buffer?.Count; j++)
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
