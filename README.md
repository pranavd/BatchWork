# Welcome to BatchWork

Simple REST service built on ASP.Net WEB API. Takes a list of url (termed as "**batch**") and processes them asynchrounously.

# Brief description
It has two endpoints, one for submitting batch and second for getting it's status.

 - **POST**  "{host}/api/submit" for sbmitting batch
	 - **Input** - takes array of valid URI.
	 - **Output** - returns **batch-id** and **batch-status**.
	 
 - **GET** "{host}/api/status" for checking status of batch
	 - **Input** - takes batch-id as input 
	 - **Output** - returns batch-status.

# Project structure

Solution is divided into two projects. **BatchWork.API** which is rest API and **BatchWork.Test** which is test project.
**Swagger** is also configured to test API from browser with the help of **SwashBuckle** library.
Path for Swagger is "{host}/swagger/ui/index" 

 - **BatchWork.API**
	 -  **DownloadController.cs**, single controller for processing both the endpoints.
	 - **BatchWoker.cs**, class file which acts as a worker class. It has follwing methods:
		 - **QueueBatchRequest**
			 - Enqueues the incoming batch request. 
		 - **ExecuteBatchRequest**
			 - Dequeues the the batch from queue and triggers the download request.
		 - **ExecuteDownloadRequest**
			 - Performs the dowload request.
		 - **GetBatchStatus**
			 - Provides the current batch-status on the basis of requested batch-id.
		 - Executes