# Singleton

オプションを付けたら色々出来るシングルトンClassです

使用サンプル

    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Extension;
    using UnityEngine.SceneManagement;
    using UnityEngine.EventSystems;

    public class SampleSceneManager : SingletonMonoBehaviour<SampleSceneManager>, ISingletonMonoBehaviourAutoCreate {

        protected Coroutine _loadScene = null;

        /// <summary>
        /// シーンの読み込み処理
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action">演出など</param>
        public void LoadScene(string sceneName, IEnumerator action = null) {
            if (_loadScene != null) throw new ArgumentException("既に読み込み中");
            _loadScene = StartCoroutine(_LoadScene(sceneName, action));
        }

        protected IEnumerator _LoadScene(string sceneName, IEnumerator action) {
            var ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            // 待機するように設定
            ao.allowSceneActivation = false;
            // 演出実行
            if(action != null)　yield return action;
            while (ao.progress < 0.9f) {
                // 次のフレームまで待機
                yield return null;
            }
            //次のレベルに遷移
            ao.allowSceneActivation = true;
            _loadScene = null;
        }
    }


    public class SampleNextSceneButton : MonoBehaviour, IPointerClickHandler {
        /// <summary>
        /// ボタンクリック時
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) {
            SampleSceneManager.Instance.LoadScene("SampleScene", Animation());
        }

        /// <summary>
        /// シーン移動前のアニメーション
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Animation() {
            Debug.Log("Animetion Start");
            yield return new WaitForSeconds(10);
            Debug.Log("Animation End");
        }
    }
