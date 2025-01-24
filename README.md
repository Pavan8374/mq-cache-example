# User Management and Recommendation System

## Overview

This project is a high-performance User Management and Recommendation System designed to handle millions of users per second. It provides features for user registration, profile management, and a sophisticated recommendation engine that suggests connections based on user interactions.

## Features

- **High Throughput**: Capable of handling millions of users and requests per second.
- **User Management**: Efficiently manage user profiles, including registration, authentication, and updates.
- **Follow System**: Users can follow each other, creating a dynamic social network.
- **Recommendation Engine**: Uses collaborative filtering and other algorithms to suggest relevant connections to users.
- **Geolocation Support**: Users can be generated with random latitude and longitude within a specified radius for location-based recommendations.
- **Bulk Data Operations**: Supports bulk user creation and follow relationships for efficient data population.

## Technologies Used

- **Backend Framework**: ASP.NET Core
- **Database**: Entity Framework Core with SQL Server (or your choice of database)
- **Data Generation**: Bogus library for generating dummy data
- **Recommendation Algorithms**: Custom algorithms for generating user recommendations
- **API Documentation**: Swagger for API documentation

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server (or any compatible database)
- [Visual Studio](https://visualstudio.microsoft.com/) or any IDE of your choice

### Installation

1. Clone the repository:

git clone [(https://github.com/Pavan8374/mq-cache-example.git)](https://github.com/Pavan8374/mq-cache-example.git)
cd mq-cache-example


2. Restore the NuGet packages:


3. Update the connection string in `appsettings.json` to point to your database.

4. Run database migrations:


5. Start the application:


### API Endpoints

#### User Management

- **Create User**
- `POST /api/user/register`
- Body: User details (JSON)

- **Seed Dummy Data**
- `POST /api/user/seed-dummy-data?userCount={count}&maxFollowsPerUser={maxFollows}&centerLatitude={lat}&centerLongitude={lon}`
- Description: Generates dummy users and follow relationships.

#### Recommendation System

- **Get Recommendations**
- `GET /api/recommendations/{userId}`
- Description: Fetches recommended users based on the specified user’s followings.

### Performance Testing

To test the performance of the system under load, you can use tools like [Apache JMeter](https://jmeter.apache.org/) or [k6](https://k6.io/). Configure these tools to simulate multiple users interacting with the API simultaneously.

### Contributing

Contributions are welcome! Please feel free to submit issues, fork the repository, and make pull requests. Ensure that you follow best practices and include tests for new features.

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to the contributors who helped make this project possible.
- Special thanks to the open-source community for providing libraries and tools that enhance development.

## Contact

For questions or feedback, please reach out via [your email] or open an issue in this repository.
