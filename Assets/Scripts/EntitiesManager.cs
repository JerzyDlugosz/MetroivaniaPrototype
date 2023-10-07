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
        foreach (var item in entities)
        {
            item.isStopped = state;
            item.stoppedEvent.Invoke(state);
        }
    }

    private void RefreshEntities()
    {
        for(int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null)
            {
                entities.RemoveAt(i);
            }
        }
    }
}
