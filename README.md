# Kafka Local Setup (No Zookeeper)

This project sets up a local Kafka broker using Docker, without needing Zookeeper.  
It includes:
- A `docker-compose.yml` file to run Kafka.
- A script to create a Kafka topic.

---

## 📦 Files

| File | Purpose |
|:-----|:--------|
| `docker-compose.yml` | Defines the Kafka container and configuration |
| `create-topic.sh` or Windows CMD command | Creates a topic inside Kafka |

---

## 🛠️ Prerequisites

- Install **Docker** and **Docker Compose** on your machine.
- Make sure Docker service is running.

---

## 🛠️ 1. How to Start Kafka

Use the Docker Compose file to start Kafka in detached mode:
`docker compose up -d`

### Explanation:

docker compose → run using Docker Compose

up → start the services

-d → run in background (detached mode)

Once running:

Kafka broker is available at localhost:29092

## 🛠️ 3. How to Create a Kafka Topic
After Kafka is running, create your topic using the following command:

docker exec kafka-standalone kafka-topics.sh --create --topic Melad --bootstrap-server localhost:29092 --partitions 3 --replication-factor 1
Explanation:

docker exec kafka-standalone → run a command inside Kafka container.

kafka-topics.sh --create → command to create a new topic.

--topic Melad → name of the topic.

--bootstrap-server localhost:29092 → connect to Kafka broker.

--partitions 3 → number of partitions.

--replication-factor 1 → only one broker copy (local dev).