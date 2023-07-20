using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPositioner : MonoBehaviour
{
    [SerializeField]
    LayerMask _walkableSurfaces;
    [SerializeField]
    Transform _ikTargetPos;
    [SerializeField]
    float _reposisitonTreshold;
    [SerializeField]
    float _maxFeetDistanceReach;

    bool _isMoving = false;
    private void Update()
    {
        PositionTarget();
        CheckDistanceToIkTarget(Time.deltaTime);
        if (_isMoving)
        {
            MoveFeetForward(Time.deltaTime);
        }
    }

    private void PositionTarget()
    {
        if (Physics.Raycast(transform.position + new Vector3(0,.1f,0),  -Vector3.up * _maxFeetDistanceReach, out RaycastHit hit, _maxFeetDistanceReach, _walkableSurfaces))
        {
            Debug.DrawRay(transform.position + new Vector3(0, .1f, 0), -Vector3.up * _maxFeetDistanceReach, Color.red, .1f);
            transform.position = hit.point;
        }
    }

    private void CheckDistanceToIkTarget(float delta)
    {
        if (Vector3.Distance(transform.position,_ikTargetPos.position) > _reposisitonTreshold)
        {
            _isMoving = true;
        }
    }

    private void MoveFeetForward(float delta)
    {
        while(Vector3.Distance(transform.position, _ikTargetPos.position) > .1f)
        {
            _ikTargetPos.position = Vector3.MoveTowards(_ikTargetPos.position, transform.position, delta);
        }
        _isMoving = false;
    }
}
