def GetVersion() {
  return new Date().format("yyyyMMdd.HHmmss", TimeZone.getTimeZone('Asia/Taipei'))
}
def imageTag = GetVersion()
pipeline{
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
	  volumeMounts:
        - mountPath: /var/lib/docker
          name: docker-graph-storage
        - mountPath: /home/jenkins/agent
          name: workspace-volume
      securityContext:
        privileged: true
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
          value: tcp://docker:2375
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
      dockerUrl = 'will1233'
      dockerRepo = 'business'

  }

  stages {
  
	stage('pull code') {
      steps {
        echo '---start pull code from git-hub---'
		echo "BRANCH_NAME = ${env.BRANCH_NAME}"
        checkout([$class: 'GitSCM', 
		  branches: [[name: "refs/heads/main"]], 
		  userRemoteConfigs: [[
			url: 'https://github.com/Will-Task/task-backend.git',
			credentialsId: 'e3f8dace-8572-41ff-9852-648dd73db06e'
		  ]],
		  extensions: []
		])

        echo '---pull code from git-hub success---'
      }
    }

    stage('通過Docker構建image') {
      steps {
		container('docker-client') {
		  sh "docker build -t ${dockerUrl}/${JOB_NAME}:${imageTag} ."
		  echo '通過Docker構建image - SUCCESS'
		}
      }
    }
    stage('將image推送到harbor') {
      steps {
	    container('docker-client') {
		  sh """docker login -u ${dockerUrl} -p  ${dockerPwd} ${dockerUrl}
                docker push ${dockerUrl}/${JOB_NAME}:${imageTag}"""
		}
		echo '將image推送到harbor - SUCCESS'
      }
	}
  }
}
