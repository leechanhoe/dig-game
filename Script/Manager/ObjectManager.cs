using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    static public ObjectManager instance;

    [SerializeField] GameObject monster4_Attack_Pf;
    [SerializeField] GameObject monster5_Attack_Pf;
    [SerializeField] GameObject monster6_Attack_Pf;
    [SerializeField] GameObject monster7_Attack1_Pf;
    [SerializeField] GameObject monster7_Attack2_Pf;
    [SerializeField] GameObject monster7_Attack3_Pf;
    [SerializeField] GameObject monster8_Attack1_Pf;
    [SerializeField] GameObject monster8_Attack2_Pf;
    [SerializeField] GameObject monster8_Attack3_Pf;
    [SerializeField] GameObject monster9_Attack_Pf;
    [SerializeField] GameObject monster0_Pf;
    [SerializeField] GameObject monster1_Pf;
    [SerializeField] GameObject monster2_Pf;
    [SerializeField] GameObject monster3_Pf;
    [SerializeField] GameObject monster4_Pf;
    [SerializeField] GameObject monster5_Pf;
    [SerializeField] GameObject monster6_Pf;
    [SerializeField] GameObject monster7_Pf;
    [SerializeField] GameObject monster8_Pf;
    [SerializeField] GameObject monster9_Pf;
    [SerializeField] GameObject bomb_Pf;
    [SerializeField] GameObject dynamite_Pf;
    [SerializeField] GameObject megaDynamite_Pf;
    [SerializeField] GameObject gas_Pf;

    GameObject[] monster4_Attack;
    GameObject[] monster5_Attack;
    GameObject[] monster6_Attack;
    GameObject[] monster7_Attack1;
    GameObject[] monster7_Attack2;
    GameObject[] monster7_Attack3;
    GameObject[] monster8_Attack1;
    GameObject[] monster8_Attack2;
    GameObject[] monster8_Attack3;
    GameObject[] monster9_Attack;
    GameObject[] monster0;
    GameObject[] monster1;
    GameObject[] monster2;
    GameObject[] monster3;
    GameObject[] monster4;
    GameObject[] monster5;
    GameObject[] monster6;
    GameObject[] monster7;
    GameObject[] monster8;
    GameObject[] monster9;
    GameObject dynamite;
    GameObject megaDynamite;
    GameObject bomb;
    GameObject[] gas;

    GameObject[] targetPool;
    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
        monster4_Attack = new GameObject[30];
        monster5_Attack = new GameObject[60];
        monster6_Attack = new GameObject[90];
        monster7_Attack1 = new GameObject[10];
        monster7_Attack2 = new GameObject[20];
        monster7_Attack3 = new GameObject[5];
        monster8_Attack1 = new GameObject[10];
        monster8_Attack2 = new GameObject[20];
        monster8_Attack3 = new GameObject[20];
        monster9_Attack = new GameObject[10];
        monster0 = new GameObject[3];
        monster1 = new GameObject[3];
        monster2 = new GameObject[3];
        monster3 = new GameObject[3];
        monster4 = new GameObject[3];
        monster5 = new GameObject[3];
        monster6 = new GameObject[3];
        monster7 = new GameObject[3];
        monster8 = new GameObject[3];
        monster9 = new GameObject[3];
        gas = new GameObject[10];

        Generate();
    }

    void Generate()
    {
        for (int i = 0; i < monster4_Attack.Length; i++)
        {
            monster4_Attack[i] = Instantiate(monster4_Attack_Pf, transform);
            monster4_Attack[i].SetActive(false);
        }

        for (int i = 0; i < monster5_Attack.Length; i++)
        {
            monster5_Attack[i] = Instantiate(monster5_Attack_Pf, transform);
            monster5_Attack[i].SetActive(false);
        }

        for (int i = 0; i < monster6_Attack.Length; i++)
        {
            monster6_Attack[i] = Instantiate(monster6_Attack_Pf, transform);
            monster6_Attack[i].SetActive(false);
        }

        for (int i = 0; i < monster7_Attack1.Length; i++)
        {
            monster7_Attack1[i] = Instantiate(monster7_Attack1_Pf, transform);
            monster7_Attack1[i].SetActive(false);
        }

        for (int i = 0; i < monster7_Attack2.Length; i++)
        {
            monster7_Attack2[i] = Instantiate(monster7_Attack2_Pf, transform);
            monster7_Attack2[i].SetActive(false);
        }

        for (int i = 0; i < monster7_Attack3.Length; i++)
        {
            monster7_Attack3[i] = Instantiate(monster7_Attack3_Pf, transform);
            monster7_Attack3[i].SetActive(false);
        }

        for (int i = 0; i < monster8_Attack1.Length; i++)
        {
            monster8_Attack1[i] = Instantiate(monster8_Attack1_Pf, transform);
            monster8_Attack1[i].SetActive(false);
        }

        for (int i = 0; i < monster8_Attack2.Length; i++)
        {
            monster8_Attack2[i] = Instantiate(monster8_Attack2_Pf, transform);
            monster8_Attack2[i].SetActive(false);
        }

        for (int i = 0; i < monster8_Attack3.Length; i++)
        {
            monster8_Attack3[i] = Instantiate(monster8_Attack3_Pf, transform);
            monster8_Attack3[i].SetActive(false);
        }

        for (int i = 0; i < monster9_Attack.Length; i++)
        {
            monster9_Attack[i] = Instantiate(monster9_Attack_Pf, transform);
            monster9_Attack[i].SetActive(false);
        }

        for (int i = 0; i < monster0.Length; i++)
        {
            monster0[i] = Instantiate(monster0_Pf, transform);
            monster0[i].SetActive(false);
        }

        for (int i = 0; i < monster1.Length; i++)
        {
            monster1[i] = Instantiate(monster1_Pf, transform);
            monster1[i].SetActive(false);
        }

        for (int i = 0; i < monster2.Length; i++)
        {
            monster2[i] = Instantiate(monster2_Pf, transform);
            monster2[i].SetActive(false);
        }

        for (int i = 0; i < monster3.Length; i++)
        {
            monster3[i] = Instantiate(monster3_Pf, transform);
            monster3[i].SetActive(false);
        }

        for (int i = 0; i < monster4.Length; i++)
        {
            monster4[i] = Instantiate(monster4_Pf, transform);
            monster4[i].SetActive(false);
        }

        for (int i = 0; i < monster5.Length; i++)
        {
            monster5[i] = Instantiate(monster5_Pf, transform);
            monster5[i].SetActive(false);
        }

        for (int i = 0; i < monster6.Length; i++)
        {
            monster6[i] = Instantiate(monster6_Pf, transform);
            monster6[i].SetActive(false);
        }

        for (int i = 0; i < monster7.Length; i++)
        {
            monster7[i] = Instantiate(monster7_Pf, transform);
            monster7[i].SetActive(false);
        }

        for (int i = 0; i < monster8.Length; i++)
        {
            monster8[i] = Instantiate(monster8_Pf, transform);
            monster8[i].SetActive(false);
        }

        for (int i = 0; i < monster9.Length; i++)
        {
            monster9[i] = Instantiate(monster9_Pf, transform);
            monster9[i].SetActive(false);
        }
        for (int i = 0; i < gas.Length; i++)
        {
            gas[i] = Instantiate(gas_Pf, transform);
            gas[i].SetActive(false);
        }
        dynamite = Instantiate(dynamite_Pf, transform);
        dynamite.SetActive(false);
        megaDynamite = Instantiate(megaDynamite_Pf, transform);
        megaDynamite.SetActive(false);
        bomb = Instantiate(bomb_Pf, transform);
        bomb.SetActive(false);
    }

    public GameObject MakeObj(string type,Vector3 position)
    {
        switch (type)
        {
            case "monster4_attack":
                targetPool = monster4_Attack;
            break;
            case "monster5_attack":
                targetPool = monster5_Attack;
                break;
            case "monster6_attack":
                targetPool = monster6_Attack;
                break;
            case "monster7_attack1":
                targetPool = monster7_Attack1;
                break;
            case "monster7_attack2":
                targetPool = monster7_Attack2;
                break;
            case "monster7_attack3":
                targetPool = monster7_Attack3;
                break;
            case "monster8_attack1":
                targetPool = monster8_Attack1;
                break;
            case "monster8_attack2":
                targetPool = monster8_Attack2;
                break;
            case "monster8_attack3":
                targetPool = monster8_Attack3;
                break;
            case "monster9_attack":
                targetPool = monster9_Attack;
                break;
            case "monster0":
                targetPool = monster0;
                break;
            case "monster1":
                targetPool = monster1;
                break;
            case "monster2":
                targetPool = monster2;
                break;
            case "monster3":
                targetPool = monster3;
                break;
            case "monster4":
                targetPool = monster4;
                break;
            case "monster5":
                targetPool = monster5;
                break;
            case "monster6":
                targetPool = monster6;
                break;
            case "monster7":
                targetPool = monster7;
                break;
            case "monster8":
                targetPool = monster8;
                break;
            case "monster9":
                targetPool = monster9;
                break;
            case "dynamite":
                dynamite.SetActive(true);
                dynamite.transform.position = position;
                return dynamite;
            case "megaDynamite":
                   megaDynamite.SetActive(true);
                megaDynamite.transform.position = position;
                return megaDynamite;
            case "bomb":
                bomb.SetActive(true);
                bomb.transform.position = position;
                return bomb;
            case "gas":
                targetPool = gas;
                break;
            default:
                break;
        }
        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                targetPool[i].transform.position = position;
                return targetPool[i];
            }
        }
        return null;
    }
}
