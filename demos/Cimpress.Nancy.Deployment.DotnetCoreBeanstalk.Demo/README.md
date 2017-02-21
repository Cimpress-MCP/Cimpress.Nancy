# Deploying a .Net Core application to AWS Elastic Beanstalk

This demo project shows off the core infrastructure needed to deploy a .Net Core application AWS Elastic Beanstalk. It assumes the following prerequisites:

1) You have an AWS account set up with Elastic Beanstalk permissions
2) You have the Elastic Beanstalk CLI installed and set up with your user
3) You have the dotnet CLI installed
4) You have am EB Windows Server application and environment with a name matching the name in .elasticbeanstalk/config.yml
5) You have a zip tool (this demo uses 7zip)

## Getting up and running

Create a .Net Core console application and set it up to host your Nancy service. Add a .elasticbeanstalk folder to the project with the configuration you expect. With this done you can simply run deploy.bat and the service will be published and deployed to AWS!
