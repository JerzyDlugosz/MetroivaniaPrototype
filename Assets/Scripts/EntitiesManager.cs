using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public List<BaseEntity> entities;

    public void AddEntity(BaseEntity entity)
    {
        RefreshEntities();
        entities.Add(entity);
    }

    public void RemoveEntity(BaseEntity entity)
    {
        RefreshEntities();
        entities.Remove(entity);
    }

    public List<BaseEntity> GetEntities()
    {
        return entities;
    }

    public void EntitiesPauseState(bool state)
    {
        RefreshEntities();
        //foreach (var item in entities)
        //{
        //    item.isStopped = state;
        //    item.stoppedEvent.Invoke(state);
        //}

        foreach (var item in entities)
        {
            item.isStopped = state;
        }

        if (state)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private void RefreshEntities()
    {
        for(int i = entities.Count - 1; i > -1; i--)
        {
            if (entities[i] == null)
            {
                entities.RemoveAt(i);
            }
        }
    }

    public void RemoveAllEntities()
    {
        RefreshEntities();
        for (int i = entities.Count - 1; i > -1; i--)
        {
            if (!entities[i].CompareTag("Player"))
            {
                entities[i].destroyEvent.Invoke();
            }
        }
    }
}
