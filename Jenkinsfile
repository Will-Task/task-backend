def GetVersion() {
  return new Date().format("yyyyMMdd.HHmmss", TimeZone.getTimeZone('Asia/Taipei'))
}
pipeline{
  agent {
    kubernetes {
      yaml """
apiVersion: v1
kind: Pod
metadata:
  labels:
    some-label: docker
spec:
  containers:
  - name: docker
    image: docker:24.0-dind
    command:
    - cat
    tty: true
    securityContext:
      privileged: true
  - name: docker-client
    image: docker:24.0-cli
    command:
    - cat
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
        checkout scmGit(branches: [[name: "${env.BRANCH_NAME}"]], extensions: [], userRemoteConfigs: [[credentialsId: 'f21bd5fe-ed95-4595-b31a-21c07fb88107', url: 'https://github.com/Will-Task/task-backend.git']])
        echo '---pull code from git-hub success---'
      }
    }

    stage('通過Docker構建image') {
      steps {
		container('docker-client') {
		  sh "docker build -t ${dockerUrl}/${JOB_NAME}:${GetVersion()} ."
		  echo '通過Docker構建image - SUCCESS'
		}
      }
    }
    stage('將image推送到harbor') {
      steps {
	    container('docker-client') {
		  sh """docker login -u ${harborUser} -p  ${harborPwd} ${harborUrl}
                docker push ${dockerUrl}/${JOB_NAME}:${tag}"""
		}
		echo '將image推送到harbor - SUCCESS'
      }
	}
  }
}
