﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace APManagerC3 {
    public static class ListExtends {
        public static void ReInsert<T>(this IList<T> list, in int newIndex, in T item) {
            // item不在集合内，抛出异常
            if (!list.Contains(item)) {
                throw new NotSupportedException("Item not in collection");
            }
            int preIndex = list.IndexOf(item);
            // 若新旧位置相同则跳过
            if (newIndex == preIndex) {
                return;
            }
            // 超左边界追加至末尾
            if (newIndex < 0) {
                list.RemoveAt(preIndex);
                list.Insert(0, item);
            }
            // 超有边界追加至末尾
            else if (newIndex > list.Count - 1) {
                list.RemoveAt(preIndex);
                list.Add(item);
            }
            // 新位置位于旧位置之前，先移除再插入
            else if (newIndex < preIndex) {
                list.RemoveAt(preIndex);
                list.Insert(newIndex, item);
            }
            // 新位置位于旧位置之后，先插入再移除
            else if (newIndex > preIndex) {
                list.Insert(newIndex, item);
                list.RemoveAt(preIndex);
            }
        }
    }

    public static class IDataObjectExtends {
        public static bool IsType(this IDataObject data, Type type) {
            return data.GetFormats().Contains(type.FullName);
        }
        public static bool IsTargetType(this IDataObject data, string dataFormat) {
            return data.GetFormats().Contains(dataFormat);
        }
    }

    public static class Util {
        public static T? FindVisualParent<T>(DependencyObject obj) where T : DependencyObject {
            while (obj != null) {
                if (obj is T) {
                    return (T)obj;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}
