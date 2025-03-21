using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    public void LoadData(Data data);

    public void SaveData(Data data);
}
