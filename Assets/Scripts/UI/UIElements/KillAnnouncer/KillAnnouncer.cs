using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class KillAnnouncer : MonoBehaviour {

    [SerializeField]
    private GameObject _killAnnouncementPrefab;
    [SerializeField]
    private float _secondsAlive = 3;
    [SerializeField]
    private Transform _topPosition;
    [SerializeField]
    private float _spacing = 30;

    private List<KillAnnouncementUI> _killAnnouncements = new List<KillAnnouncementUI>();

    private void OnEnable() {

        
        ClearAnnouncements();
    }

    public void CreateKillAnnouncement(string left, string right, Sprite middleIcon) {

        if (!isActiveAndEnabled)
            return;

        var obj = Instantiate(_killAnnouncementPrefab, transform);
        Debug.Log("Create kill announcement");
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
            _topPosition.transform.localPosition.y - _spacing * (_killAnnouncements.Count + 1),
            obj.transform.localPosition.z);

        var killAnnouncement = obj.GetComponent<KillAnnouncementUI>();
        killAnnouncement.SetKillAnnouncement(left,right, middleIcon);
        _killAnnouncements.Add(killAnnouncement);

        Move();
        StartCoroutine(DestroyAfterDelay(killAnnouncement));
    }

    private IEnumerator DestroyAfterDelay(KillAnnouncementUI killAnnouncement) {

        yield return new WaitForSeconds(_secondsAlive);

        _killAnnouncements.Remove(killAnnouncement);
        DOTween.Kill(killAnnouncement.gameObject);
        Destroy(killAnnouncement.gameObject);

        Move();
    }

    private void Move() {

        if (_killAnnouncements.Count <= 0)
            return;

        for (int i = 0; i < _killAnnouncements.Count; i++) {

            _killAnnouncements[i].transform.DOLocalMoveY(_topPosition.transform.localPosition.y - _spacing * i, 1);
        }
    }

    private void ClearAnnouncements() {

        foreach(var announcement in _killAnnouncements) {

            Destroy(announcement.gameObject);
        }
        _killAnnouncements.Clear();
    }

    private void OnDisable() {

        ClearAnnouncements();
    }
}
