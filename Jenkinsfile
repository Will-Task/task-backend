def GetVersion() {
  return new Date().format("yyyyMMdd.HHmmss", TimeZone.getTimeZone('Asia/Taipei'))
}
def imageTag = GetVersion()

pipeline {
  agent {
    kubernetes {
      yaml """
apiVersion: v1
kind: Pod
metadata:
  namespace: devops-tools
  serviceAccount: 'jenkins-admin'
  labels:
    some-label: docker
spec:
  volumes:
    - name: docker-graph-storage
      emptyDir: {}
    - name: workspace-volume
      emptyDir: {}
  containers:
    - name: docker
      image: docker:24.0-dind
      securityContext:
        privileged: true
      resources:
        requests:
          memory: "512Mi"
          cpu: "500m"
        limits:
          memory: "2Gi"
          cpu: "1"
      volumeMounts:
        - mountPath: /var/lib/docker
          name: docker-graph-storage
        - mountPath: /home/jenkins/agent
          name: workspace-volume
      env:
        - name: DOCKER_TLS_CERTDIR
          value: ""
      command:
        - dockerd-entrypoint.sh
      args:
        - --host=tcp://0.0.0.0:2375
        - --host=unix:///var/run/docker.sock
      tty: true
    - name: docker-client
      image: docker:24.0-cli
      command:
        - cat
      env:
        - name: DOCKER_HOST
          value: tcp://localhost:2375
      tty: true
    - name: kubectl
      image: bitnami/kubectl:latest
      command:
        - cat
      tty: true
  restartPolicy: Never
"""
    }
  }

  environment {
    dockerUser = 'will1233'
    dockerPwd = 'Az@98198506'
    dockerRegistry = 'will1233'  // 這裡要填你的 Harbor registry domain or IP
    dockerRepo = 'business'
  }

  stages {
    stage('pull code') {
      steps {
        echo '---start pull code from git-hub---'
        checkout([
          $class: 'GitSCM',
          branches: [[name: "refs/heads/main"]],
          userRemoteConfigs: [[
            url: 'https://github.com/Will-Task/task-backend.git',
            credentialsId: 'e3f8dace-8572-41ff-9852-648dd73db06e'
          ]]
        ])
        echo '---pull code from git-hub success---'
      }
    }

    stage('通過Docker構建image') {
      steps {
        container('docker-client') {
          sh "docker build -f MicroServices/Business/Dockerfile -t ${dockerRegistry}/business1:${imageTag} MicroServices/Business"
          echo '通過Docker構建image - SUCCESS'
        }
      }
    }

    stage('將image推送到harbor') {
      steps {
        container('docker-client') {
          sh """
            docker login -u ${dockerUser} -p ${dockerPwd}
            docker push ${dockerRegistry}/business1:${imageTag}
          """
        }
        echo '將image推送到harbor - SUCCESS'
      }
    }
  }
}
