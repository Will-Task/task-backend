def GetVersion() {
  return new Date().format("yyyyMMdd.HHmmss", TimeZone.getTimeZone('Asia/Taipei'))
}
pipeline{
  agent any

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
        script {
          env.imageTag = GetVersion()
          echo "Image tag: ${env.imageTag}"
        }
		echo '---pull code from git-hub success---'
      }
    }

    stage('通過Docker構建image') {
      steps {
		sh "docker build -t ${dockerUrl}/${JOB_NAME}:${env.imageTag} ."
        echo '通過Docker構建image - SUCCESS'
      }
    }
    stage('將image推送到harbor') {
      steps {
          sh """echo $dockerPwd | docker login -u $dockerUser --password-stdin
                docker push ${dockerUrl}/${JOB_NAME}:${env.imageTag}"""
        echo '將image推送到harbor - SUCCESS'
      }
	}
  }
}
