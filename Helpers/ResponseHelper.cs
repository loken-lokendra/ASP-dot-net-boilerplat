using Microsoft.AspNetCore.Http;

namespace ASPDOTNETDEMO.Helpers
{
    public static class ResponseHelper
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Request successful")
        {
            return new ApiResponse<T>(StatusCodes.Status200OK, true, message, data);
        }

        public static ApiResponse<T> Created<T>(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T>(StatusCodes.Status201Created, true, message, data);
        }

        public static ApiResponse<T> Fail<T>(string message, int statusCode = StatusCodes.Status400BadRequest)
        {
            return new ApiResponse<T>(statusCode, false, message, default);
        }

        public static ApiResponse<T> Unauthorized<T>(string message = "Unauthorized access")
        {
            return new ApiResponse<T>(StatusCodes.Status401Unauthorized, false, message, default);
        }

        public static ApiResponse<T> NotFound<T>(string message = "Resource not found")
        {
            return new ApiResponse<T>(StatusCodes.Status404NotFound, false, message, default);
        }

        public static ApiResponse<T> ServerError<T>(string message = "An unexpected error occurred")
        {
            return new ApiResponse<T>(StatusCodes.Status500InternalServerError, false, message, default);
        }
    }
}
