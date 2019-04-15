# Image comparer

Simple web application for comparing images and detecting differences between them

## Getting Started

App is built on .NET Core 3.0 and is crossplatform

### Prerequisites

[.NET Core and ASP.NET Core v3.0.0-preview3](https://dotnet.microsoft.com/download/dotnet-core/3.0)

### Build
```
cd ImageComparer/
dotnet build
```

### Test
```
cd ImageComparer/ImageComparerTests/
dotnet test
```

### Run
Application starts on https://localhost:5001 and http://localhost:5000
```
cd ImageComparer/ImageComparer/
dotnet run
```

## API

### Find differences between two images
Compares two images, finds differences between them and put these differences in red rectangles. Rectangles are drawn on the first image provided.

* **URL**

  /api/differences

* **Method**

  `POST`  
 
* **URL Params**

   None

* **Data Params**

  `files` - two images in form-data body
    
* **Success Response**

   First image with red rectangles around zones different from the second image
   
### Start processing two images in the background
Starts a task of processing two images in the background without waiting for completion.

* **URL**

  /api/differences/task

* **Method**

  `POST`  
 
* **URL Params**

   None

* **Data Params**

  `files` - two images in form-data body
    
* **Success Response**

   ID of the created task (GUID as string)
   
### Check status of a task
Checks status of the task started with /api/differences/task request.

* **URL**

  /api/differences/task/{guid}/status

* **Method**

  `GET`  
 
* **URL Params**

   **Required:**
 
   `guid=[string]`

* **Data Params**

  None
    
* **Success Response**

   State of a given task - `InProgress` or `Completed`
    
### Get the result of a background task
Gets the result of a background task, either complete or not.

* **URL**

  /api/differences/task/{guid}

* **Method**

  `GET`  
 
* **URL Params**

   **Required:**
 
   `guid=[string]`

* **Data Params**

  None
    
* **Success Response**

   First image with red rectangles around zones different from the second image. If called on the incomplete task, shows all found differences to this moment.
   
   
 ## Design approach
 The task of showing a difference between two images was split into sub-tasks - finding areas in which images differ; drawing these areas; storing the result. All these tasks are controlled by `ImageComparerManager`.
 
 ### Finding differences between images
 Algorithms that find differences between two images should implement `IImageComparerAlgorithm` interface. This interface contains two methods - `GetDifferences` and `GetDifferencesAsync` - for synchronous and asynchronous processing. `GetDifferencesAsync` returns `IAsyncEnumerable`, a new feature of C# 8.0, that allows lazy asynchronous enumeration of returned collection.
 
 Two algorithms are implemented - `PixelByPixelImageComparerAlgorithm` and `GridImageComparerAlgorithm`.
 
 `PixelByPixelImageComparerAlgorithm` compares every pixel of both images with custom comparer that should implement `IPixelComparerAlgorithm`.  For example, `ArgbPixelComparerAlgorithm` compares pixels by comparing their ARGB values, with error tolerance.
 
 `GridImageComparerAlgorithm` resizes both images to smaller ones, i.e. 32x32, and calls `PixelByPixelImageComparerAlgorithm` for those resized images - comparing not every pixel of original images, but "hashes" of them.
 
 ### Drawing and storing
 `ImagePainter`, that implements `IImagePainter`, simply draws provided rectangles, both in sync and async ways.
 
 It may be desirable to save comparison results - i.e. looking at previous comparisons can help the user to set right tolerance value - so, simple `InnerImageStorage`, that implements `IImageStorage`, is provided. This class stores images in static ConcurrentDictionary.
 
 ### Asynchronous operations
 All calls to `Bitmap` properties in async context must be protected with locks, so locks are passed as parameters in image comparers algorithms and also stored in image storage.
 
 Using `yield return` inside a lock in the method that returns `IAsyncEnumerable` results in locking the resource until the collection is enumerated. Modifying images during comparison doesn't make sense, but read access is also blocked.
 
 To show the progress of work, `ImagePainter` works with a copy of the first image and has his own locks for this copy. Later, this copy (and lock too) is stored.
 
 `IAsyncEnumerable` and `await foreach` enables easy progress tracking. After any difference is found by `PixelByPixelImageComparerAlgorithm`, that difference is transformed in `GridImageComparerAlgorithm` and then is painted by `ImagePainter`. After that `PixelByPixelImageComparerAlgorithm` checks next area, and so on.
 
 ## Strong features
 * Abstraction makes it easy to extend existing methods - i.e. use HSV or HSL values instead of ARBG for comparing pixels, or using clustering techniques to group differences instead of using the grid.
 * `IAsyncEnumerable` provides a simple and robust solution for tracking progress
 * Running tasks in the background makes it possible to load multiple pairs of images and getting the comparison result of each pair as soon as it is ready
 
 ## Weak features
 * Fire-and-forget at `ImageComparerManager.ProcessInBackground` is a temporary solution and doesn't allow to control running tasks or catch exceptions in them.
 * Unsuccessful response of API is not specified, no exceptions are catched
 * Some settings, like grid size or color of rectangles around differences, can't be set by the user
 
 ## Ways to improve
 * Using `Bitmap.LockBits` instead of `Bitmap.GetPixel`, as it is much faster
 * Implementing task queue in `ImageComparerManager`
 * Creating another endpoint in API for general settings
