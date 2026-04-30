using UnityEngine;


// 생성된 방 <-> 내부 컨텐츠 배치를 연결하는 로직
public class RoomInstanceExample : MonoBehaviour
{
    [Header("Referance")]
    // 이 방의 타입 (Combat, Shop 등)
    public RoomGraphExample.RoomType roomType;

    // 방 내부 콘텐츠를 생성하는 스크립트 참조
    private RoomContentSpawner contentsSpawner;

    private void Awake()
    {
        // 같은 GameObject에 붙어있는 RoomContentSpawner 가져오기
        contentsSpawner = GetComponent<RoomContentSpawner>();
    }

    /// <summary>
    /// 외부(RoomGraphExample)에서 호출되는 초기화 함수
    /// 방 타입을 설정하고 내부 콘텐츠 생성을 시작함
    /// </summary>
    /// <param name="type">이 방의 타입 (Combat, Shop 등)</param>
    public void Init(RoomGraphExample.RoomType type)
    {
        roomType = type;
        
        //// RoomContentSpawner가 있으면 콘텐츠 생성 실행
        //if(contentsSpawner != null)
        //{
        //    contentsSpawner.GenerateContent(roomType);
        //}

    }

}
