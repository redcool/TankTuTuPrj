using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 敌人生成器 - 管理波次系统和敌人生成
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        // 序列化字段
        [Header("生成设置")]
        [SerializeField] private int _waveCount = 10;
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private float _waveDelay = 5f;

        [Header("敌人预制体")]
        [SerializeField] private GameObject _beaverPrefab;
        [SerializeField] private GameObject _cowPrefab;
        [SerializeField] private GameObject _elephantBossPrefab;

        [Header("生成区域")]
        [SerializeField] private Vector2 _spawnArea = new Vector2(20, 20);
        [SerializeField] private int _maxEnemies = 50;

        // 私有字段
        private int _currentWave = 0;
        private int _enemiesSpawned = 0;
        private int _enemiesKilled = 0;
        private bool _isSpawning = false;
        private List<GameObject> _activeEnemies = new List<GameObject>();

        // 公有属性
        public int CurrentWave => _currentWave;
        public int EnemiesKilled => _enemiesKilled;
        public int TotalEnemies => _waveCount;

        /// <summary>
        /// 海狸预制体（setter供SceneInitializer调用）
        /// </summary>
        public GameObject BeaverPrefab
        {
            get => _beaverPrefab;
            set => _beaverPrefab = value;
        }

        /// <summary>
        /// 奶牛预制体（setter供SceneInitializer调用）
        /// </summary>
        public GameObject CowPrefab
        {
            get => _cowPrefab;
            set => _cowPrefab = value;
        }

        /// <summary>
        /// 大象Boss预制体（setter供SceneInitializer调用）
        /// </summary>
        public GameObject ElephantBossPrefab
        {
            get => _elephantBossPrefab;
            set => _elephantBossPrefab = value;
        }

        /// <summary>
        /// 开始生成波次
        /// </summary>
        public void StartSpawning()
        {
            if (_isSpawning) return;
            StartCoroutine(SpawnWaves());
        }

        /// <summary>
        /// 停止生成
        /// </summary>
        public void StopSpawning()
        {
            _isSpawning = false;
            StopAllCoroutines();
        }

        /// <summary>
        /// 重置生成器
        /// </summary>
        public void ResetSpawner()
        {
            StopSpawning();
            _currentWave = 0;
            _enemiesSpawned = 0;
            _enemiesKilled = 0;
            _activeEnemies.Clear();
        }

        /// <summary>
        /// 生成波次协程
        /// </summary>
        private IEnumerator SpawnWaves()
        {
            _isSpawning = true;

            for (_currentWave = 1; _currentWave <= _waveCount; _currentWave++)
            {
                yield return StartCoroutine(SpawnWave(_currentWave));

                // 波次间延迟
                if (_currentWave < _waveCount)
                {
                    yield return new WaitForSeconds(_waveDelay);
                }
            }

            // 等待所有敌人消灭
            yield return new WaitUntil(() => _activeEnemies.Count == 0);

            // 波次完成
            OnWavesComplete();
        }

        /// <summary>
        /// 生成单个波次
        /// </summary>
        private IEnumerator SpawnWave(int waveNumber)
        {
            // 根据波次计算敌人生成数量
            int enemyCount = Mathf.Min(3 + waveNumber, _maxEnemies);
            bool spawnBoss = (waveNumber % 5 == 0);  // 每5波生成Boss

            for (int i = 0; i < enemyCount; i++)
            {
                if (!_isSpawning) yield break;

                // 检查最大敌人数量
                if (_activeEnemies.Count >= _maxEnemies)
                {
                    yield return new WaitUntil(() => _activeEnemies.Count < _maxEnemies);
                }

                // 生成敌人
                GameObject enemyPrefab = GetEnemyPrefab(waveNumber, spawnBoss && i == 0);
                if (enemyPrefab != null)
                {
                    SpawnEnemy(enemyPrefab);
                }

                // 生成间隔
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        /// <summary>
        /// 根据波次获取敌人预制体
        /// </summary>
        private GameObject GetEnemyPrefab(int waveNumber, bool forceBoss = false)
        {
            if (forceBoss && _elephantBossPrefab != null)
            {
                return _elephantBossPrefab;
            }

            // 随机选择小怪
            if (_beaverPrefab != null && _cowPrefab != null)
            {
                return Random.value > 0.5f ? _beaverPrefab : _cowPrefab;
            }

            return _beaverPrefab;
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        private void SpawnEnemy(GameObject prefab)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            _activeEnemies.Add(enemy);
            _enemiesSpawned++;

            // 注册死亡回调
            var enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                // 敌人死亡时从列表移除
                StartCoroutine(RegisterDeathCallback(enemy));
            }
        }

        /// <summary>
        /// 注册死亡回调
        /// </summary>
        private IEnumerator RegisterDeathCallback(GameObject enemy)
        {
            // 简单的轮询检测敌人是否存活
            yield return new WaitUntil(() => enemy == null || !enemy.activeInHierarchy);

            if (enemy != null && _activeEnemies.Contains(enemy))
            {
                _activeEnemies.Remove(enemy);
                _enemiesKilled++;
            }
        }

        /// <summary>
        /// 获取随机生成位置
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            // 在玩家周围生成，但保持一定距离
            Vector3 playerPos = Vector3.zero;

            var tanks = FindObjectsOfType<TankController>();
            if (tanks.Length > 0)
            {
                // 计算所有战车的中心位置
                Vector3 center = Vector3.zero;
                foreach (var tank in tanks)
                {
                    center += tank.transform.position;
                }
                playerPos = center / tanks.Length;
            }

            // 随机生成位置
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(5f, Mathf.Max(_spawnArea.x, _spawnArea.y));

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );

            return playerPos + offset;
        }

        /// <summary>
        /// 波次完成回调
        /// </summary>
        private void OnWavesComplete()
        {
            _isSpawning = false;
            Debug.Log("[EnemySpawner] 所有波次完成！击杀数: " + _enemiesKilled);

            // 通知GameManager
            GameManager.Instance?.OnWavesComplete();
        }

        /// <summary>
        /// 获取当前存活敌人数量
        /// </summary>
        public int GetActiveEnemyCount()
        {
            // 清理已销毁的敌人
            _activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
            return _activeEnemies.Count;
        }
    }
}
