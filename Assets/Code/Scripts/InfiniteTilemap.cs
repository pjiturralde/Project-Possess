using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class InfiniteTilemap : MonoBehaviour {
    private PlayerManager playerManager;
    private Transform player;
    private Rigidbody2D rb;
    private int mapSizeX;
    private int mapSizeY;

    void Start() {
        playerManager = PlayerManager.instance;
        player = playerManager.transform;
        rb = player.GetComponent<Rigidbody2D>();
        mapSizeX = 132;
        mapSizeY = 87;
    }

    void Update() {

    }

    void FixedUpdate() {
        bool isSpearAttacking = false; // spear literally breaks this

        foreach (Transform child in player) {
            if (child.gameObject.CompareTag("PlayerWeapon")) {
                if (child.GetComponent<Spear>() != null) {
                    Spear spear = child.GetComponent<Spear>();

                    isSpearAttacking = spear.attacking;
                }
            }
        }

        if (!isSpearAttacking) {
            if (player.position.y >= mapSizeY / 2) {
                rb.linearVelocity = Vector3.zero;
                player.position = new Vector3(player.position.x, player.position.y - mapSizeY, 0);
                TeleportEverything(new Vector3(0, -mapSizeY, 0));
            }

            if (player.position.y <= -mapSizeY / 2) {
                rb.linearVelocity = Vector3.zero;
                player.position = new Vector3(player.position.x, player.position.y + mapSizeY, 0);
                TeleportEverything(new Vector3(0, mapSizeY, 0));
            }

            if (player.position.x >= mapSizeX / 2) {
                rb.linearVelocity = Vector3.zero;
                player.position = new Vector3(player.position.x - mapSizeX, player.position.y, 0);
                TeleportEverything(new Vector3(-mapSizeX, 0, 0));
            }

            if (player.position.x <= -mapSizeX / 2) {
                rb.linearVelocity = Vector3.zero;
                player.position = new Vector3(player.position.x + mapSizeX, player.position.y, 0);
                TeleportEverything(new Vector3(mapSizeX, 0, 0));
            }
        }
    }

    public void TeleportEverything(Vector3 offset) {
        TeleportAllEnemies(offset);
        TeleportAllProjectiles(offset);
        TeleportAllWeapons(offset);
        TeleportAllItems(offset);
        TeleportWizard(offset);
    }

    public void TeleportAllEnemies(Vector3 offset) {
        List<GameObject> enemies = new List<GameObject>();

        enemies.AddRange(GameObject.FindGameObjectsWithTag("ArmedEnemy"));
        enemies.AddRange(GameObject.FindGameObjectsWithTag("UnarmedEnemy"));
        enemies.AddRange(GameObject.FindGameObjectsWithTag("RangedEnemy"));

        foreach (GameObject enemy in enemies) {
            Transform enemyTransform = enemy.transform;

            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            enemyTransform.position = enemyTransform.position + offset;
        }
    }

    public void TeleportAllProjectiles(Vector3 offset) {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        foreach (GameObject projectile in projectiles) {
            Transform projectileTransform = projectile.transform;

            projectileTransform.position = projectileTransform.position + offset;
        }
    }

    public void TeleportAllWeapons(Vector3 offset) {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("FreeWeapon");

        foreach (GameObject weapon in weapons) {
            Transform weaponTransform = weapon.transform;

            weaponTransform.position = weaponTransform.position + offset;
        }
    }

    public void TeleportAllItems(Vector3 offset) {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items) {
            Transform itemTransform = item.transform;

            itemTransform.position = itemTransform.position + offset;
        }
    }

    public void TeleportWizard(Vector3 offset) {
        GameObject wizard = GameObject.FindGameObjectWithTag("Wizard");

        wizard.transform.position = wizard.transform.position + offset;
    }
}
