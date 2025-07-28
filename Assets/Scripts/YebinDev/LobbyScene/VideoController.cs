using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public RawImage mScreen = null;
    public VideoPlayer mVideoPlayer = null;

    
    public RawImage mScreen2 = null;
    public VideoPlayer mVideoPlayer2 = null;

    public RawImage mScreen3 = null;
    public VideoPlayer mVideoPlayer3 = null;

    
    public RawImage mScreen4 = null;
    public VideoPlayer mVideoPlayer4 = null;

    public RawImage mScreen5 = null;
    public VideoPlayer mVideoPlayer5 = null;

    void Start()
    {
        if (mScreen != null && mVideoPlayer != null)
        {
            StartCoroutine(PrepareVideo());
        }

        if (mScreen2 != null && mVideoPlayer2 != null)
        {
            StartCoroutine(PrepareVideo());
        }
        
        if (mScreen3 != null && mVideoPlayer3 != null)
        {
            StartCoroutine(PrepareVideo());
        }

        if (mScreen4 != null && mVideoPlayer4 != null)
        {
            StartCoroutine(PrepareVideo());
        }

        if (mScreen5 != null && mVideoPlayer5 != null)
        {
            StartCoroutine(PrepareVideo());
        }
    }

    protected IEnumerator PrepareVideo()
    {
        // ���� �غ�
        mVideoPlayer.Prepare();
        mVideoPlayer2.Prepare();
        mVideoPlayer3.Prepare();
        mVideoPlayer4.Prepare();
        mVideoPlayer5.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!mVideoPlayer.isPrepared && !mVideoPlayer2.isPrepared
        && !mVideoPlayer3.isPrepared && !mVideoPlayer4.isPrepared && !mVideoPlayer5.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer�� ��� texture�� RawImage�� texture�� �����Ѵ�
        mScreen.texture = mVideoPlayer.texture;
        mScreen2.texture = mVideoPlayer2.texture;
        mScreen3.texture = mVideoPlayer3.texture;
        mScreen4.texture = mVideoPlayer4.texture;
        mScreen5.texture = mVideoPlayer5.texture;
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // ���� ���
            mVideoPlayer.Play();
        }

        if (mVideoPlayer2 != null && mVideoPlayer2.isPrepared)
        {
            // ���� ���
            mVideoPlayer2.Play();
        }
        if (mVideoPlayer3 != null && mVideoPlayer3.isPrepared)
        {
            // ���� ���
            mVideoPlayer3.Play();
        }
        if (mVideoPlayer4 != null && mVideoPlayer4.isPrepared)
        {
            // ���� ���
            mVideoPlayer4.Play();
        }
        if (mVideoPlayer5 != null && mVideoPlayer5.isPrepared)
        {
            // ���� ���
            mVideoPlayer5.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // ���� ����
            mVideoPlayer.Stop();
        }

        if (mVideoPlayer2 != null && mVideoPlayer2.isPrepared)
        {
            // ���� ����
            mVideoPlayer2.Stop();
        }
        
        if (mVideoPlayer3 != null && mVideoPlayer3.isPrepared)
        {
            // ���� ����
            mVideoPlayer3.Stop();
        }
        if (mVideoPlayer4 != null && mVideoPlayer4.isPrepared)
        {
            // ���� ����
            mVideoPlayer4.Stop();
        }
        if (mVideoPlayer5 != null && mVideoPlayer5.isPrepared)
        {
            // ���� ����
            mVideoPlayer5.Stop();
        }
    }
}
