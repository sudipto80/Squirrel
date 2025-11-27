namespace Squirrel;

/// <summary>
/// Represents comprehensive outlier detection algorithms covering univariate, multivariate,
/// machine learning, time series, and statistical test-based approaches for identifying
/// anomalous data points across various data types and distributions.
/// </summary>
public enum OutlierDetectionAlgorithm
{
    #region Univariate Statistical Methods
    
    /// <summary>
    /// Represents the Interquartile Range (IQR) Interval algorithm, which detects outliers by
    /// identifying values that fall outside a specified range based on the IQR, calculated
    /// as the difference between the 75th percentile (Q3) and the 25th percentile (Q1).
    /// </summary>
    IqrInterval,
    
    /// <summary>
    /// Specifies the Z-Score algorithm, which identifies outliers based on the number
    /// of standard deviations a data point deviates from the mean of the dataset.
    /// </summary>
    ZScore,
    
    /// <summary>
    /// Represents the Modified Z-Score algorithm, which identifies outliers
    /// by measuring data deviations from the median, scaled using the Median Absolute
    /// Deviation (MAD). This algorithm is effective for detecting outliers in data
    /// distributions that may not follow normality, offering a robust alternative to
    /// mean-based methods such as standard deviation or traditional Z-Score.
    /// </summary>
    ModifiedZScore,
    
    /// <summary>
    /// Represents the Percentile-based algorithm, which identifies outliers
    /// by determining if values fall outside specific percentile boundaries.
    /// Suitable for analyzing datasets where extreme values are defined relative
    /// to their position within a statistical percentile distribution.
    /// </summary>
    Percentile,
    
    /// <summary>
    /// Represents the Standard Deviation-based algorithm, which identifies
    /// outliers by evaluating how far data points deviate from the mean
    /// beyond a specific multiple of the standard deviation. This approach
    /// is particularly suitable for datasets with a normal (Gaussian) distribution.
    /// </summary>
    StandardDeviation,
    
    /// <summary>
    /// Represents the Hampel Identifier algorithm, which detects outliers using
    /// a sliding window approach based on the median and Median Absolute Deviation (MAD).
    /// Particularly effective for time series data and robust against multiple outliers.
    /// </summary>
    HampelIdentifier,
    
    #endregion
    
    #region Statistical Tests
    
    /// <summary>
    /// Represents Grubbs' test (also known as the maximum normalized residual test),
    /// which detects a single outlier in a univariate dataset assumed to be normally distributed.
    /// Tests whether the most extreme value is significantly different from the rest.
    /// </summary>
    GrubbsTest,
    
    /// <summary>
    /// Represents Dixon's Q-test, which identifies outliers in small datasets (typically n &lt; 30)
    /// by comparing the gap between suspected outliers and their nearest neighbors
    /// to the overall range of the data.
    /// </summary>
    DixonTest,
    
    /// <summary>
    /// Represents the Rosner test (Generalized Extreme Studentized Deviate test),
    /// which can detect multiple outliers simultaneously in normally distributed data,
    /// addressing the masking problem where multiple outliers hide each other.
    /// </summary>
    RosnerTest,
    
    /// <summary>
    /// Represents the Tietjen-Moore test, which tests for multiple outliers
    /// when the number of outliers is specified in advance. Useful for detecting
    /// a known number of potential outliers in normally distributed data.
    /// </summary>
    TietjenMooreTest,
    
    #endregion
    
    #region Multivariate Methods
    
    /// <summary>
    /// Represents the Mahalanobis Distance algorithm, which detects outliers in
    /// multivariate data by measuring the distance of each observation from the
    /// dataset's centroid, accounting for the covariance structure of the data.
    /// Effective for identifying outliers in multi-dimensional space.
    /// </summary>
    MahalanobisDistance,
    
    /// <summary>
    /// Represents PCA-based outlier detection, which identifies anomalies by
    /// projecting data onto principal components and detecting observations
    /// with unusually large reconstruction errors or extreme scores on principal components.
    /// </summary>
    PrincipalComponentAnalysis,
    
    /// <summary>
    /// Represents the Minimum Covariance Determinant (MCD) algorithm, which provides
    /// robust estimates of location and scatter for multivariate data and identifies
    /// outliers based on their robust Mahalanobis distances. Particularly effective
    /// when the dataset contains a significant proportion of outliers.
    /// </summary>
    MinimumCovarianceDeterminant,
    
    /// <summary>
    /// Represents the Robust Principal Component Analysis approach for outlier detection,
    /// which identifies anomalies using robust estimates of principal components
    /// that are less sensitive to outliers than traditional PCA methods.
    /// </summary>
    RobustPCA,
    
    /// <summary>
    /// Represents Hotelling's T-squared test for multivariate outlier detection,
    /// which identifies observations that are significantly distant from the
    /// multivariate mean using the T-squared statistic.
    /// </summary>
    HotellingTSquared,
    
    #endregion
    
    #region Machine Learning Approaches
    
    /// <summary>
    /// Represents the Isolation Forest algorithm, which isolates outliers by
    /// randomly selecting features and split values, creating isolation trees.
    /// Outliers require fewer splits to be isolated, making this method effective
    /// for high-dimensional data and capable of handling large datasets efficiently.
    /// </summary>
    IsolationForest,
    
    /// <summary>
    /// Represents One-Class Support Vector Machine (SVM), which learns a decision
    /// boundary around normal data points in a high-dimensional feature space.
    /// Effective for novelty detection and outlier identification in complex,
    /// non-linear data patterns.
    /// </summary>
    OneClassSVM,
    
    /// <summary>
    /// Represents the Local Outlier Factor (LOF) algorithm, which detects outliers
    /// based on the local density deviation of data points relative to their neighbors.
    /// Particularly effective for identifying outliers in datasets with varying densities
    /// and local clustering patterns.
    /// </summary>
    LocalOutlierFactor,
    
    /// <summary>
    /// Represents DBSCAN-based outlier detection, which identifies outliers as noise points
    /// that don't belong to any cluster formed by the DBSCAN clustering algorithm.
    /// Effective for datasets with irregular cluster shapes and varying densities.
    /// </summary>
    DBSCANBased,
    
    /// <summary>
    /// Represents the Elliptic Envelope algorithm, which fits a robust covariance
    /// estimate to the data and identifies outliers based on their Mahalanobis distance
    /// from the fitted elliptic envelope. Assumes data follows a Gaussian distribution.
    /// </summary>
    EllipticEnvelope,
    
    /// <summary>
    /// Represents K-Nearest Neighbors (KNN) based outlier detection, which identifies
    /// outliers based on the distance to their k-th nearest neighbor or the average
    /// distance to their k nearest neighbors. Simple yet effective for many datasets.
    /// </summary>
    KNearestNeighbors,
    
    /// <summary>
    /// Represents autoencoder-based outlier detection using neural networks,
    /// which identifies anomalies based on reconstruction error. Data points that
    /// cannot be well reconstructed by the trained autoencoder are considered outliers.
    /// </summary>
    Autoencoder,
    
    #endregion
    
    #region Time Series Specific
    
    /// <summary>
    /// Represents seasonal decomposition-based outlier detection, which identifies
    /// anomalies by decomposing time series into trend, seasonal, and residual components,
    /// then detecting outliers in the residual component after removing trend and seasonality.
    /// </summary>
    SeasonalDecomposition,
    
    /// <summary>
    /// Represents ARIMA residual-based outlier detection, which fits an ARIMA model
    /// to the time series and identifies outliers based on standardized residuals
    /// that exceed specified thresholds. Effective for time series with trend and seasonality.
    /// </summary>
    ARIMAresidualsReplication,
    
    /// <summary>
    /// Represents Seasonal and Trend decomposition using Loess (STL) based outlier detection,
    /// which decomposes time series using robust regression and identifies outliers
    /// in the remainder component. Particularly robust to outliers in the decomposition process.
    /// </summary>
    STLDecomposition,
    
    /// <summary>
    /// Represents exponential smoothing-based outlier detection, which uses exponential
    /// smoothing models to forecast expected values and identifies observations that
    /// deviate significantly from predictions as potential outliers.
    /// </summary>
    ExponentialSmoothing,
    
    /// <summary>
    /// Represents change point detection-based outlier identification, which identifies
    /// structural breaks or regime changes in time series that may represent
    /// systemic outliers or anomalous periods rather than individual data points.
    /// </summary>
    ChangePointDetection,
    
    #endregion
    
    #region Ensemble and Advanced Methods
    
    /// <summary>
    /// Represents ensemble-based outlier detection, which combines multiple
    /// outlier detection algorithms to improve robustness and accuracy by
    /// aggregating results from diverse detection methods.
    /// </summary>
    EnsembleMethod,
    
    /// <summary>
    /// Represents histogram-based outlier detection, which identifies outliers
    /// based on the frequency distribution of data points. Points falling in
    /// bins with very low frequencies are considered potential outliers.
    /// </summary>
    HistogramBased,
    
    /// <summary>
    /// Represents clustering-based outlier detection using various clustering algorithms
    /// (K-means, hierarchical, etc.) where outliers are identified as points that
    /// are far from cluster centers or form very small clusters.
    /// </summary>
    ClusteringBased,
    
    /// <summary>
    /// Represents angle-based outlier detection (ABOD), which identifies outliers
    /// based on the variance of angles between a point and all other points.
    /// Particularly effective in high-dimensional spaces where distance-based methods may fail.
    /// </summary>
    AngleBased,
    
    #endregion
}