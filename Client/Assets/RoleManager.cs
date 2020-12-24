using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour {
    public List<Role> m_roles = new List<Role>();
    
    public void SetHealth(int id, int amount)
    {
        if(m_roles.Count > id)
        {
            m_roles[id].Health = amount;
        }
    }

    public void SetWisdom(int id, int amount)
    {
        if (m_roles.Count > id)
        {
            m_roles[id].Wisdom = amount;
        }
    }

    public void SetAgility(int id, int amount)
    {
        if (m_roles.Count > id)
        {
            m_roles[id].Agility = amount;
        }
    }
    
}