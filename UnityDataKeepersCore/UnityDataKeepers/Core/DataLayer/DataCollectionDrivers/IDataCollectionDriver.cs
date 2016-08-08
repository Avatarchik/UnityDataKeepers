﻿using System.Collections.Generic;
using UnityDataKeepersCore.Core.DataLayer.RevisionData;
using UnityEngine;

namespace UnityDataKeepersCore.Core.DataLayer.DataCollectionDrivers
{
    public interface IDataCollectionDriver<TItem>
        where TItem : class, IDataItem
    {
        TItem GetByHash(Hash128 hash);
        bool Add(TItem item);
        bool Remove(TItem item);
        int Add(IEnumerable<TItem> items);
        int Remove(IEnumerable<TItem> items);
        bool Update(TItem item);
        IEnumerable<TItem> GetAll();
        void Clear();
        int Count();
    }
}