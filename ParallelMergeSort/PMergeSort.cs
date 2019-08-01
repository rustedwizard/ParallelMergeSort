using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelMergeSort
{
    public static class PMergeSort<T> where T : IComparable
    {
        //Main sorting algorithm
        //IEnumerable<T> input: the sequence need to be sorted
        //bool ascending: if true (by default), indicate the sorting will be in asending order,
        //                if false, sorting will be in decending order
        //threashold: the size or partition (by default threashold = 100). 
        //            This algorithm effectively divode the list into several partition, 
        //            and sort each partition in parallel and merge them back into one list
        //            therefore a partition size is need.
        public static IEnumerable<T> Sort(IEnumerable<T> toSort, bool ascending = true, int threashold = 100)
        {
            //When only one element left, indicate that
            //the divide is complete return the list.
            T[] enumerable = toSort as T[] ?? toSort.ToArray();
            if (enumerable.Count() == 1)
            {
                return enumerable;
            }
            //Calculate the mid point
            //Merge method will handle the 
            //length difference in case of odd number of element
            var mid = enumerable.Count() / 2;
            //Construct two new list
            //containing left half and right half of original list
            //and recursively sort it.
            List<T> left = new List<T>();
            List<T> right = new List<T>();
            //if list contains more elements than given threashold amount
            //introduce parallel process into sorting
            if (enumerable.Count() > threashold)
            {
                Task t = Task.Run(() =>
                {
                    left = Callback(enumerable, mid, true, ascending);
                });
                Task t2 = Task.Run(() =>
                {
                    right = Callback(enumerable, mid, false, ascending);
                });
                t.Wait();
                t2.Wait();
            }
            //if list is short or the recursive call reduced the list to a small
            //enough size, then perform the sorting in MergeSort algorithm.
            else
            {
                left = Callback(enumerable, mid, true, ascending);
                right = Callback(enumerable, mid, false, ascending);
            }
            //after divide is complete
            //merge the list and return the result
            return Merge(left, right, ascending);
        }
       
        //private helper method
        //the main function for this method is to separate the list
        //form given mid point and rescursively call Sort method
        public static List<T> Callback(T[] mainList, int mid, bool left, bool ascending)
        {
            List<T> toSort = new List<T>();
            //left: bool indicating program is processing left half or right half
            if (left)
            {
                //left half list
                for(var i=0; i<mid; i++)
                {
                    toSort.Add(mainList.ElementAt(i));
                }
            }
            else
            {
                //right half list
                for(var i = mid; i < mainList.Count(); i++)
                {
                    toSort.Add(mainList.ElementAt(i));
                }
            }
            return Sort(toSort.AsEnumerable(), ascending).ToList();
        }

        public static List<T> Merge(List<T> left, List<T> right, bool ascending)
        {
            var result = new List<T>();
            //Check for left and right half list for empty list
            while (left.Count > 0 || right.Count > 0)
            {
                //if none of two lists are empty,
                //then, proceed with normal merge
                if (left.Count > 0 && right.Count > 0)
                {
                    //if ascending order is requested
                    //proceed with add the smaller element first
                    if (ascending)
                    {
                        if (left[0].CompareTo(right[0]) <= 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                    else
                    //if ascending order is not requested
                    //then sorting for descending order
                    //proceed with add the larger element first
                    {
                        if (left[0].CompareTo(right[0]) >= 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                }
                //if right half list is empty
                //copy the rest of left half list
                else if (left.Count > 0)
                {
                    foreach (var item in left)
                    {
                        result.Add(item);
                    }
                    left = new List<T>();
                }
                //if left half list is empty
                //copy the rest of right half list
                else
                {
                    foreach (var item in right)
                    {
                        result.Add(item);
                    }
                    right = new List<T>();
                }
            }
            //return final sorted list
            return result;
        }
    }
}
