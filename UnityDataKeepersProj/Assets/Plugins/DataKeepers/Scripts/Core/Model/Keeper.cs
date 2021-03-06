﻿using System.Collections.Generic;
using System;
using System.Collections;
using DataKeepers.DataBase;
using UnityEngine;
using UnityEngine.Events;

namespace DataKeepers
{
    [Serializable]
    public class Keeper<TKeeper, TItem> : SerializableCharpSingleton<TKeeper>, IEnumerable<TItem> where TKeeper : class where TItem : KeeperItem, new()
    {
        private static DataKeepersDbConnector _dataConnector = null;

        protected Keeper()
        {
            if (_dataConnector == null)
            {
                _dataConnector = new DataKeepersDbConnector();
                _dataConnector.ConnectToLocalStorage();
            }
            if (!_dataConnector.TableExists<TItem>())
            {
                _dataConnector.CreateTable<TItem>();
            }
        }

        public KeeperItemEvent OnAddItem = new KeeperItemEvent();
        public KeeperItemEvent OnDeleteItem = new KeeperItemEvent();

        public TItem GetById(string id)
        {
            var result = _dataConnector.GetQuery<TItem>(i=>i.Id == id);
            return result.Count > 0 ? result[0] : null;
        }

        public List<TItem> GetAllById(string id)
        {
            return _dataConnector.GetQuery<TItem>(i=>i.Id == id);
        }

        public List<TItem> GetAll()
        {
            return FindAll(i => true);
        }

        public TItem Find(Predicate<TItem> predicate)
        {
            var all = FindAll(predicate);
            if (all.Count < 1) return null;
            return all[0];
        }

        public List<TItem> FindAll(Predicate<TItem> predicate)
        {
            return _dataConnector.GetQuery(predicate);
        }

        public virtual bool Add(TItem item)
        {
            if (!Validate(item)) return false;
            try
            {
                _dataConnector.Insert(item);
            }
            catch (Exception e)
            {
                Debug.Log("Can't insert object " + item.Justify() + " because error: " + e.Message);
                return false;
            }
            return true;
        }

        public virtual int Add(IEnumerable<TItem> items)
        {
            if (items == null) return 0;
            int c = 0;
            foreach (var i in items)
                if (Add(i)) c++;
            return c;
        }

        public virtual bool Remove(TItem i)
        {
            try
            {
                _dataConnector.Remove(i);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual int Remove(IEnumerable<TItem> items)
        {
            var c = 0;
            foreach (var i in items)
                if (Remove(i)) c++;
            return c;
        }

        protected virtual bool Validate(TItem obj)
        {
            return true;
        }

        public int Count()
        {
            try
            {
                return _dataConnector.GetCount<TItem>();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public virtual void Clear()
        {
            try
            {
                _dataConnector.DeleteAll<TItem>();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public virtual void Update(TItem item)
        {
            try
            {
                _dataConnector.Update(item);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error when updating object {0}: {1}", item.Justify(), e.Message));
                // ignored
            }
        }

        public virtual void InsertOrUpdate(TItem item)
        {
            try
            {
                if (_dataConnector.Update(item) == 0)
                    _dataConnector.Insert(item);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error when updating object {0}: {1}", item.Justify(), e.Message));
                // ignored
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return FindAll(i => true).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class KeeperItemEvent : UnityEvent<KeeperItem>
    {
    
    }
}