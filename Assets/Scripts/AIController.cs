using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AIController : MonoBehaviour
{
    public float speed = 10f;
    public float distance = 3f;
    //private bool isSearching = true;
    private int[] direction = new int[4];


    //add 이전블록이 분기형이었는지 체크용 변수
    //public int check_before_weight;

    //add stuck 블럭 체크를 위한 4방향 디텍트 가중치 합계 but!!!!4방향이아니라 4개중 3개의합이 0일시 or 0이 3개일시
    public int direct_sum = 0;

    //add
    List<int> rand_route = new List<int>();

    GameObject target;
    //add
    GameObject target_now;

    //add
    private int[] direction_check = new int[4];


    private int[] waypoint = new int[10000];
    private int waycount = 0;

    private void Start()
    {
        InvokeRepeating("CalcDistance", 1.0f, 0.7f);
        InvokeRepeating("Moving", 1.5f, 0.7f);
    }

    void CalcDistance()
    {
        //0,1,2,3 --> 동서남북
        direction[0] = ShootRayEast();
        direction[1] = ShootRayWest();
        direction[2] = ShootRaySouth();
        direction[3] = ShootRayNorth();

        //add
        //foreach (int s in direction)
        //{
        //    direct_sum = direct_sum + s;
        //}

        //add  4방면이 모두 0인경우는 무한루프를 예방한 예외로 추후에 처리!!!! 예를들어 goto goal 이라던가 goto 마지막 분기점이라던가
        //add  or 4방면이 모두 0인경우에 arctan(observer height/7.5) 로 계산해서 각도를 줄여 2칸뒤 plane 참조
        //add  or 4방면일 경우에 한해 4방면 ray 해서 Plane이면 일단이동(Plane만 골라서 이동하다보면 벽을 뚫지도 않을것이고, 가다보면 언젠가 weight 가 0이아닌곳을 찾을것. 그러면 다시 루프 탈출
        Array.Copy(direction, direction_check, 4);

        for (int i = 0; i < 3; i++)
            direct_sum = direct_sum + direction_check[i];

        //add
        if (direct_sum == 0)
        {
            int weight_now = 0;
            RaycastHit hit;
            var direction_now = Quaternion.Euler(new Vector3(90, 0, 0)) * transform.forward;
            Ray ray_now = new Ray(transform.position, direction_now);
            if (Physics.Raycast(transform.position, direction_now, out hit, distance))
            {
                if (hit.collider.tag == "Plane")
                {
                    target_now = hit.collider.gameObject;
                    target_now.GetComponent<PlaneWeight>().weight = 0;
                }
                else { weight_now = 0; }
            }
            Debug.Log(weight_now);
        }
        else
        {
            direct_sum = 0;
        }


        Debug.Log("거리 배열 : ");
        for (int i = 0; i < 4; i++)
            Debug.Log(direction[i]);

        int max = direction[0];
        for (int i = 1; i < 4; i++)
        {
            if (direction[i] >= max) max = direction[i];
        }
        for (int i = 0; i < 4; i++)
        {
            if (direction[i] == max)
            {
                Debug.Log("최댓값 : " + max);

                //add
                rand_route.Add(i);

                //delete
                //루트정보에 동서남북으로 루트 표시
                //waypoint[waycount] = i;
            }
        }

        //add
        waypoint[waycount] = rand_route[UnityEngine.Random.Range(0, rand_route.Count)];
        rand_route.Clear();


        Debug.Log("향할 방향 : " + waypoint[waycount]);
    }

    void Moving()
    {
        //루트대로 오브젝트 움직여주기
        if (waypoint[waycount] == 0)
        {
            ShootRayEast();
            Debug.Log("Way to East");
            transform.Translate(new Vector3(5, 0, 0));
            target.GetComponent<PlaneWeight>().decreaseWeight();
        }
        else if (waypoint[waycount] == 1)
        {
            ShootRayWest();
            Debug.Log("Way to West");
            transform.Translate(new Vector3(-5, 0, 0));
            target.GetComponent<PlaneWeight>().decreaseWeight();
        }
        else if (waypoint[waycount] == 2)
        {
            ShootRaySouth();
            Debug.Log("Way to South");
            transform.Translate(new Vector3(0, 0, -5));
            target.GetComponent<PlaneWeight>().decreaseWeight();
        }
        else if (waypoint[waycount] == 3)
        {
            ShootRayNorth();
            Debug.Log("Way to North");
            transform.Translate(new Vector3(0, 0, 5));
            target.GetComponent<PlaneWeight>().decreaseWeight();
        }
        waycount++;

        //add
        //check_before_weight = target.GetComponent<PlaneWeight>().getWeight();
    }


    public int ShootRayNorth()
    {
        int weight = 0;
        RaycastHit hit;
        var directionNorth = Quaternion.Euler(new Vector3(20, 0, 0)) * transform.forward;
        Ray rayNorth = new Ray(transform.position, directionNorth);
        //Debug.DrawLine(rayNorth.origin, rayNorth.origin + rayNorth.direction * distance, Color.red);
        if (Physics.Raycast(transform.position, directionNorth, out hit, distance))
        {
            if (hit.collider.tag == "Plane")
            {
                //Debug.Log("North!");
                target = hit.collider.gameObject;
                weight = target.GetComponent<PlaneWeight>().getWeight();
            }
            else { weight = 0; }
        }
        Debug.Log(weight);
        return weight;
    }

    public int ShootRaySouth()
    {

        int weight = 0;
        RaycastHit hit;
        var directionSouth = Quaternion.Euler(new Vector3(20, 180, 0)) * transform.forward;
        Ray raySouth = new Ray(transform.position, directionSouth);
        //Debug.DrawLine(raySouth.origin, raySouth.origin + raySouth.direction * distance, Color.blue);
        if (Physics.Raycast(transform.position, directionSouth, out hit, distance))
        {
            if (hit.collider.tag == "Plane")
            {
                //Debug.Log("South!");
                target = hit.collider.gameObject;
                weight = target.GetComponent<PlaneWeight>().getWeight();
            }
            else { weight = 0; }
        }
        Debug.Log(weight);
        return weight;
    }
    public int ShootRayEast()
    {
        int weight = 0;
        RaycastHit hit;
        var directionEast = Quaternion.Euler(new Vector3(20, 90, 0)) * transform.forward;
        Ray rayEast = new Ray(transform.position, directionEast);
        //Debug.DrawLine(rayEast.origin, rayEast.origin + rayEast.direction * distance, Color.green);
        if (Physics.Raycast(transform.position, directionEast, out hit, distance))
        {
            if (hit.collider.tag == "Plane")
            {
                //Debug.Log("East!");
                target = hit.collider.gameObject;
                weight = target.GetComponent<PlaneWeight>().getWeight();
            }
            else { weight = 0; }
        }
        Debug.Log(weight);
        return weight;
    }
    public int ShootRayWest()
    {
        int weight = 0;
        RaycastHit hit;
        var directionWest = Quaternion.Euler(new Vector3(20, 270, 0)) * transform.forward;
        Ray rayWest = new Ray(transform.position, directionWest);
        //Debug.DrawLine(rayWest.origin, rayWest.origin + rayWest.direction * distance, Color.black);
        if (Physics.Raycast(transform.position, directionWest, out hit, distance))
        {
            if (hit.collider.tag == "Plane")
            {
                //Debug.Log("West!");
                target = hit.collider.gameObject;
                weight = target.GetComponent<PlaneWeight>().getWeight();
            }
            else { weight = 0; }
        }
        Debug.Log(weight);

        return weight;
    }


}
