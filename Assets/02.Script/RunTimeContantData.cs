
using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;

public class RunTimeContantData
{

}


public enum RunState // 게임상태
{
    Title,
    Adventure,
    Battle
}

[System.Serializable]
public class PlayerState // 플레이어 상태
{
    public int hp;
    public Vector2 position;
    public List<string> deck; // 카드 ID
}


[System.Serializable]
public class MapState // 맵 정보
{
    public int seed;
    public List<string> deadEnemies;
}
