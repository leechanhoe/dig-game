using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public bool DropBomb() // 폭탄투하
    {
        if (isDigging || isFlying || Bomb.usingBomb)
            return false;
        ObjectManager.instance.MakeObj("bomb", transform.position);
        return true;
    }

    public bool DropDynamite(bool mega = false) // 다이너마이트 투하
    {
        if (isDigging || isFlying || Bomb.usingBomb)
            return false;
        Ground scanGround = scanDownObject.GetComponent<Ground>();
        if (scanGround.haveRock || scanGround.haveHardRock || scanGround.haveSpecialRock)
        {
            GameObject dynamite;
            if(!mega)
                dynamite = ObjectManager.instance.MakeObj("dynamite", scanDownObject.transform.position);
            else
                dynamite = ObjectManager.instance.MakeObj("megaDynamite", scanDownObject.transform.position);
            dynamite.GetComponent<Dynamite>().target = scanDownObject;
        }
        else
        {
            GameObject dynamite = Instantiate(dynamitePrefab, transform.position, Quaternion.Euler(Vector3.zero));
            dynamite.GetComponent<Dynamite>().target = null;
        }
        return true;
    }

    Vector3 storedPosition; // 어드밴스드 텔레포트 스톤으로 저장된 위치
    bool isStored;
    public bool UseAdTeleportStone1() // 어드밴스드 텔레포트 스톤사용해서 위치저장할떄
    {
        if (isDigging || isFlying || Bomb.usingBomb || isStored)
            return false;
        storedPosition = transform.position;
        isStored = true;
        Inventory inventory = FindObjectOfType<Inventory>();
        DatabaseManager.instance.ReturnItem(10203).itemDescription += playerStat.depth + "m";
        inventory.GetAnItem(10203,floatText : false);
        inventory.ShowItem();
        return true;
    }

    public void RemoveStoredPos()
    {
        isStored = false;
    }

    public bool UseAdTeleportStone2() // 어드밴스드 텔레포트 스톤사용해서 위치저장할떄
    {
        if (isDigging || isFlying || Bomb.usingBomb || !isStored)
            return false;
        transform.position = storedPosition;
        RemoveStoredPos();
        DatabaseManager.instance.ReturnItem(20203).itemDescription = "현재 위치를 저장한다. 다시 저장한 위치로 이동할 수 있다. 저장된 위치 : 지하 ";
        return true;
    }
}
