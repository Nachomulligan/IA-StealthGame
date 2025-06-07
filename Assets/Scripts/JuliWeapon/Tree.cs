using UnityEngine;

public class Tree : MonoBehaviour
{
    public int life;
    public int bullets;
    public bool lineOfSight;

    ITreeNode _root;

    void Start()
    {
        InitializedTree();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _root.Execute();
        }
    }
    void InitializedTree()
    {
        ITreeNode shoot = new ActionNode(Shoot);
        ITreeNode reload = new ActionNode(() =>
        {
            print("Reload");
            bullets += 5;
        }
        );
        ITreeNode patrol = new ActionNode(Patrol);
        ITreeNode died = new ActionNode(Died);

        ITreeNode qHasBullet = new QuestionNode(QuestionHasBullet, shoot, reload);
        ITreeNode qNeedReload = new QuestionNode(QuestionHasBullet, patrol, reload);
        ITreeNode qLineOfSight = new QuestionNode(QuestionLineOfSight, qHasBullet, qNeedReload);
        ITreeNode qHalfLife = new QuestionNode(() => life > 0, qLineOfSight, died);

        _root = qHalfLife;
    }

    bool QuestionLineOfSight()
    {
        return lineOfSight;
    }
    bool QuestionHasBullet()
    {
        return bullets > 0;
    }
    void Shoot()
    {
        bullets--;
        print("Shoot");
    }
    void Patrol()
    {
        print("Patrol");
    }
    void Died()
    {
        print("Died");
    }
}
