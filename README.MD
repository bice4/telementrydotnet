# Telemetry in .NET

This project is dedicated to implementing a robust telemetry solution in .NET, focusing on centralizing logs, collecting metrics, and tracing the flow of operations within the application. The goal is to enhance observability, streamline issue detection, and optimize overall system performance. The solution involves using dedicated collectors and visualizers for each telemetry aspect.

## Solution 1: Centralized Logging using Seq

* [LoggingAllInOne](logging/LoggingAllInOne/README.md)

Overview:

This solution focuses on centralizing logs using Seq, a powerful log server. It provides a structured approach to logging, making it easier to analyze and trace issues within the application.

***Key Features:***

1) **Seq Integration:**
    * Utilizes the Seq log server to centralize logs from different components.
    * Enhances log readability and searchability with Seq's intuitive user interface.
2) **Serilog Configuration:**
   * Demonstrates how to configure Serilog for structured logging.
   * Integrates Serilog with Seq for seamless log transmission.
3) **Centralized Log Exploration:**
   * Guides users on exploring and querying centralized logs using Seq.

## Solution 2: Metrics, Tracing, Logging

* [TelemetryNetProject](telemetry/TelemetryNetProject/README.md)

Overview:
This solution provides a more holistic approach to telemetry, covering metrics, tracing, and logging. It incorporates industry-standard tools like OpenTelemetry, Prometheus, Grafana, and Jaeger to achieve comprehensive observability.

***Key Features:***

1) **OpenTelemetry Integration:**
   * Configures OpenTelemetry to capture traces within the .NET application.
   * Enables distributed tracing for monitoring the flow of operations.
  
2) **Prometheus Metrics:**
    * Illustrates how to instrument the application with Prometheus metrics using the `prometheus-net` library.
    * Sets up a MetricServer to expose metrics for collection.
  
3) **Grafana Dashboard:**

    * Integrates Grafana with Prometheus to visualize and explore metrics.
    * Integrates Grafana with Loki to collect and show logs.
    * Creates a dashboard for monitoring key performance indicators.
  
4) **Jaeger Tracing:**

    * Incorporates Jaeger as the tracing backend.
    * Guides users in exploring traces and understanding the application's request flow.
  
5) **Combined Telemetry:**

   * Demonstrates the coexistence of metrics, tracing, and logging within the application.
   * Provides a unified approach to monitoring and diagnosing issues.


## Benefits of the Repository:

* **Educational Resource:**

  * Serves as an educational resource for developers seeking to implement robust telemetry practices in .NET applications.
  
* **Real-world Examples:**

  * Offers real-world examples of integrating popular telemetry tools, making it easier for developers to adapt these practices to their projects.
  
* **Comprehensive Observability:**

  * Addresses different facets of observability, including logging, metrics, and tracing, to provide a well-rounded understanding of telemetry.
  
* **Ease of Adoption:**
  * Each solution includes clear instructions and configurations, making it straightforward for developers to adopt and customize based on their project requirements.

Feel free to explore each solution in detail using the provided links to gain hands-on experience with telemetry in .NET. Whether you are a beginner or an experienced developer, these examples aim to enhance your understanding of building observable and maintainable .NET applications.
