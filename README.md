# Welcome to BatchWork

Simple REST service built on ASP.Net WEB API. Takes a list of url (termed as "**batch**") and processes them asynchrounously.

# Brief Description
It has two endpoints, one for submitting batch and second for getting it's status.

 - **POST**  "{host}/api/submit" for sbmitting batch
	 - **Input** - takes array of valid url.
	 - **Output** - returns **batch-id** and **[batch-status](#batch-status)**.
	 
 - **GET** "{host}/api/status" for checking status of batch
	 - **Input** - takes batch-id as input 
	 - **Output** - returns batch-status.

# Project Structure

Solution is divided into two projects. **BatchWork.API** which is REST API and **BatchWork.Test** which is test project.
**Swagger** is also configured to test API from browser with the help of **SwashBuckle** library.
Path for Swagger is "{host}/swagger/ui/index" 

 - **BatchWork.API**
	 -  **DownloadController**, single controller for processing both the endpoints.
		 - **SubmitBatch**, enqueues the batch request and triggers for async download
		 - **GetBatchStatus**, provides the batch status on the basis of batch-id.
	 - **WebApiApplication**, inherits from **System.Web.HttpApplication**. Declares necessary data structures for API to work.
		 - **BatchDictionary**, stores the batch-status for corresponding batch-id
		 - **UrlQueue**, stores the incoming batch request in queued manner.
		 - **ContentBag**, stores the downloaded content from url.
	 - **BatchWoker.cs**, class file which acts as a worker class. It has follwing methods:
		 - **QueueBatchRequest**
			 - Enqueues the incoming batch request to **BatchDictionary** and **UrlQueue**.
		 - **ExecuteBatchRequest**
			 - Dequeues the the batch from **UrlQueue** and triggers the download request and updates the batch-status in **BatchDictionary**.
		 - **ExecuteDownloadRequest**
			 - Performs the dowload request and stores the downloaded content in **ContentBag**.
		 - **GetBatchStatus**
			 - Provides the current batch-status on the basis of requested batch-id.
	 - **Models**
		 - **BatchModel**, model class for storing batch request with corresponding list of url
		 - **DownloadedContentModel**, model class for storing downloaded content from url
		 - **RequestModel**, model class for incoming url
		 - <a href ="#" name="batch-status">**StatusEnum**</a>, Enum which denotes the **batch-status**. Consist of  status COMPLETED, INPROGRESS, QUEUED, FAILED.
 - **BatchWork.Test** 
	 - **BatchWorkApiTest**, contains UTCs for API. It is following methods:
		 - **Test_Action_SubmitBatch**, UTC for  **SubmitBatch** action.
		 - **Test_Action_GetBatchStatustext**, UTC for **GetBatchStatus** action.
		 - **Test_Action_GetBatchStatus_Completed**, UTC for completed batches.

## Dependencies

 - .Net Framework 4.6.1
 - Visual Studio 2017
 - Git
 - SwashBuckle

# Notes

## Steps to Run

 - Clone the repo
 - Open **BatchWork.sln** in VS2017
 - F6 (Build Solution)
 - For running API, press F5
	 - Open **http://localhost:4098/swagger/ui/index** in the browser
 - For running UTCs
	 - If UTCs are not discovered, please delete **.vs** folder in solution directory and re-build the solution.

## Glossary

 - **Batch**, each request with list of url to download is considered as Batch.
 - **batch-id**, GUID representing unique id for every Batch
