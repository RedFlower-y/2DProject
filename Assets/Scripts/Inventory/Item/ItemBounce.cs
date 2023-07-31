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
        /// ��ʼ����Ʒʱ�ĳ�ʼ��
        /// </summary>
        /// <param name="target">Ŀ��λ��</param>
        /// <param name="dir">����</param>
        public void InitBounceItem(Vector3 target,Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = target;
            distance = Vector3.Distance(target, transform.position);
            itemSpriteTrans.position += Vector3.up * 1.5f;              // �ӳ�����Ʒ����Ϊ����ʱ��Ʒ������(�����ܽ���Ʒsprite��Ӱ�ӵ�sprite�ֿ�)
        }

        /// <summary>
        /// ��Ʒ�ڱ��ӹ��̵��˶�
        /// </summary>
        private void Bounce()
        {
            isGround = itemSpriteTrans.position.y <= transform.position.y;
            if(Vector3.Distance(transform.position,targetPos)>0.1f)
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;     // ����Խ�����屻��ʱ���ٶ�Խ��
            }

            if(!isGround)
            {
                // ��û�е��䵽������
                itemSpriteTrans.position += Vector3.up * gravity * Time.deltaTime;  // ��Ʒ��sprite��y�������˶�(Ӱ�Ӳ���Ҫ����Ϊ������Ǵ�����ŵ׵�Ŀ������ŵ�)
            }
            else
            {
                // ���䵽������
                itemSpriteTrans.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}
