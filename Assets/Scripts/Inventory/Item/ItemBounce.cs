using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform itemSpriteTrans;
        private BoxCollider2D coll;
        public float gravity = -3.5f;
        private bool isGround;
        private float distance;
        private Vector2 direction;
        private Vector3 targetPos;

        private void Awake()
        {
            itemSpriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        /// <summary>
        /// 开始扔物品时的初始化
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="dir">方向</param>
        public void InitBounceItem(Vector3 target,Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = target;
            distance = Vector3.Distance(target, transform.position);
            itemSpriteTrans.position += Vector3.up * 1.5f;              // 扔出的物品坐标为举着时物品的坐标(这样能将物品sprite和影子的sprite分开)
        }

        /// <summary>
        /// 物品在被扔过程的运动
        /// </summary>
        private void Bounce()
        {
            isGround = itemSpriteTrans.position.y <= transform.position.y;
            if(Vector3.Distance(transform.position,targetPos)>0.1f)
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;     // 距离越大物体被扔时的速度越快
            }

            if(!isGround)
            {
                // 还没有掉落到地面上
                itemSpriteTrans.position += Vector3.up * gravity * Time.deltaTime;  // 物品的sprite的y轴向下运动(影子不需要，因为本身就是从人物脚底到目标坐标脚底)
            }
            else
            {
                // 掉落到地面上
                itemSpriteTrans.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}
