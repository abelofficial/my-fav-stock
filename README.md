# Stock Price Scraper

This project is a serverless application built with .NET 6 and AWS SAM (Serverless Application Model) that scrapes Yahoo Finance for stock prices and other information. It retrieves stock/index data for a list of symbols configured in the Dato CMS, and stores the results in an S3 bucket in a structured format.

## Project Overview

-   **Language**: .NET 6
-   **Serverless Framework**: AWS SAM
-   **Purpose**: Scrape stock price data from Yahoo Finance every 5 minutes and store it in an S3 bucket.

## Features

-   **Scheduled Lambda Trigger**: The Lambda function is triggered every 5 minutes.
-   **Data Source**: Yahoo Finance for stock and index prices.
-   **Configuration**: A list of stock symbols is pulled from Dato CMS.
-   **Data Storage**: Scraped data is saved in an S3 bucket under the format:
    s3://bucket-name/index-name/date/time.json

## Project Structure

-   **Lambda Function**: Located in `LambdaFunctions::LambdaFunctions.Functions.StockScraperFunc`. The function fetches data from Yahoo Finance, processes it, and stores it in the specified S3 bucket.
-   **S3 Bucket**: Stores JSON data files with pricing information.
-   **AWS SAM Template**: The `template.yaml` file defines the serverless resources, including the Lambda function and permissions.

## Getting Started

### Prerequisites

-   **AWS CLI**: Install and configure with credentials and permissions.
-   **AWS SAM CLI**: Install to build and deploy the serverless application.
-   **.NET 6 SDK**: Install for building and packaging the Lambda function.

### Setup Instructions

1. **Clone the Repository**:

```bash
git clone <repository-url>
cd <repository-folder>
```

2. Build the Project: Use the SAM CLI to build the .NET project:

```bash
Copy code
sam build
```

3. Configure Environment Variables: Update environment variables for AWS credentials, S3 bucket name, and Dato CMS API credentials, if applicable.

Deploy the Application: Deploy using the SAM CLI:

```bash
Copy code
sam deploy --guided
```

4. Follow the prompts to configure deployment settings, such as stack name, region, and S3 bucket name for storing data.

### SAM Template Details

The SAM template (template.yaml) provisions the following resources:

Lambda Function (ScrapStockValues): Scrapes data from Yahoo Finance every 5 minutes and writes it to an S3 bucket.
IAM Role: Grants the Lambda function read access to Dato CMS and read/write access to the specified S3 bucket.
Scheduled Event: Triggers the Lambda function every 5 minutes.
Data Storage Structure
The scraped data is saved in the S3 bucket in the following format:

```php
Copy code
s3://<bucket-name>/<index-name>/<date>/<time>.json
```

Where:

-   bucket-name is the specified S3 bucket.
-   index-name is the name of the stock or index being scraped.
-   date and time are the date and time of the data fetch, used to organize data historically.

### Testing

To test the function locally:

```bash
Copy code
sam local invoke ScrapStockValues
```

This command simulates an invocation of the Lambda function locally.

### Cleanup

To delete the stack and all associated resources, run:

```bash
Copy code
sam delete
```

### License

This project is licensed under the MIT License. See the LICENSE file for details. Enjoy scraping stock prices and building insights!
