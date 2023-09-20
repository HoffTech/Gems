// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;

namespace Gems.Metrics
{
    /// <summary>
    /// Интерфес для работы с метриками.
    /// </summary>
    public interface IMetricsService
    {
        /// <summary>
        /// Создание датчика и увеличение на increment.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Gauge(string name, string description, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание датчика и увеличение на increment.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Gauge(string name, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание датчика и увеличение на increment.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Gauge(Enum enumValue, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание датчика и увеличение на increment.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Gauge(MetricInfo metricInfo, double increment = 1);

        /// <summary>
        /// Устанавливает значение датчика в targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeSet(string name, string description, double targetValue, params string[] labelValues);

        /// <summary>
        /// Устанавливает значение датчика в targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeSet(string name, double targetValue, params string[] labelValues);

        /// <summary>
        /// Устанавливает значение датчика в targetValue.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeSet(Enum enumValue, double targetValue, params string[] labelValues);

        /// <summary>
        /// Устанавливает значение датчика в targetValue.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeSet(MetricInfo metricInfo, double targetValue);

        /// <summary>
        /// Увеличивает значение датчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeIncTo(string name, string description, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение датчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeIncTo(string name, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение датчика до targetValue.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeIncTo(Enum enumValue, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение датчика до targetValue.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeIncTo(MetricInfo metricInfo, double targetValue);

        /// <summary>
        /// Уменьшает значение датчика на decrement.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="decrement">Значение декремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDec(string name, string description, double decrement = 1, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика на decrement.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="decrement">Значение декремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDec(string name, double decrement = 1, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика на decrement.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="decrement">Значение декремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDec(Enum enumValue, double decrement = 1, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика на decrement.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="decrement">Значение декремента.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDec(MetricInfo metricInfo, double decrement = 1);

        /// <summary>
        /// Уменьшает значение датчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDecTo(string name, string description, double targetValue, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDecTo(string name, double targetValue, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика до targetValue.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDecTo(Enum enumValue, double targetValue, params string[] labelValues);

        /// <summary>
        /// Уменьшает значение датчика до targetValue.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task GaugeDecTo(MetricInfo metricInfo, double targetValue);

        /// <summary>
        /// Создание счетчика и увеличение на increment.
        /// </summary>
        /// <param name="name">Имя метрики.</param>
        /// <param name="description">Описание метрики.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Counter(string name, string description, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание счетчика и увеличение на increment.
        /// </summary>
        /// <param name="name">Имя метрики.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Counter(string name, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание счетчика и увеличение на increment.
        /// </summary>
        /// <param name="enumValue">Имя метрики.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Counter(Enum enumValue, double increment = 1, params string[] labelValues);

        /// <summary>
        /// Создание счетчика и увеличение на increment.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="increment">Значение инкремента.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Counter(MetricInfo metricInfo, double increment = 1);

        /// <summary>
        /// Увеличивает значение счетчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task CounterTo(string name, string description, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение счетчика до targetValue.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task CounterTo(string name, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение счетчика до targetValue.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task CounterTo(Enum enumValue, double targetValue, params string[] labelValues);

        /// <summary>
        /// Увеличивает значение счетчика до targetValue.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="targetValue">Финальное значение метрики.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task CounterTo(MetricInfo metricInfo, double targetValue);

        /// <summary>
        /// Создание гистограммы и публикует значение метрики.
        /// </summary>
        /// <param name="name">Имя метрики.</param>
        /// <param name="description">Описание метрики.</param>
        /// <param name="value">Значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Histogram(string name, string description, double value, params string[] labelValues);

        /// <summary>
        /// Создание гистограммы и публикует значение метрики.
        /// </summary>
        /// <param name="name">Имя метрики.</param>
        /// <param name="value">Значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Histogram(string name, double value, params string[] labelValues);

        /// <summary>
        /// Создание гистограммы и публикует значение метрики.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="value">Значение метрики.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Histogram(Enum enumValue, double value, params string[] labelValues);

        /// <summary>
        /// Создание гистограммы и публикует значение метрики.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <param name="value">Значение метрики.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task Histogram(MetricInfo metricInfo, double value);

        /// <summary>
        /// Создает метрику времени.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="description">Описание.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        TimeMetric Time(string name, string description, params string[] labelValues);

        /// <summary>
        /// Создает метрику времени.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        TimeMetric Time(string name, params string[] labelValues);

        /// <summary>
        /// Создает метрику времени.
        /// </summary>
        /// <param name="enumValue">Значение перечисления.</param>
        /// <param name="labelValues">Значения меток.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        TimeMetric Time(Enum enumValue, params string[] labelValues);

        /// <summary>
        /// Создает метрику времени.
        /// </summary>
        /// <param name="metricInfo">Информация о метрике.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        TimeMetric Time(MetricInfo metricInfo);

        /// <summary>
        /// Сбрасывает метрики по таймерам.
        /// </summary>
        Task ResetMetrics();
    }
}
