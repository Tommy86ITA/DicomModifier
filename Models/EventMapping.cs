﻿// Models/EventMapping.cs

namespace DicomModifier.Models
{
    public static class EventMapping
    {
        public enum EventType
        {
            AuditDBCreated,
            DefaultAdminCreated,

            LoginSuccessful,
            LoginFailed_InvalidCredentials,
            LoginFailed_UserDisabled,
            LoginFailed_SystemError,

            PasswordChanged,

            UserCreated,
            UserDeleted,

            UserDisabled,
            UserEnabled,

            // Aggiungi altri tipi di eventi qui
        }

        public enum EventSeverity
        {
            Informational,
            Warning,
            Error
        }

        public static readonly Dictionary<EventType, EventSeverity> _eventSeverityMap = new()
        {
        { EventType.AuditDBCreated, EventSeverity.Informational },
        { EventType.DefaultAdminCreated, EventSeverity.Informational },

        { EventType.LoginSuccessful, EventSeverity.Informational },
        { EventType.LoginFailed_InvalidCredentials, EventSeverity.Warning },
        { EventType.LoginFailed_UserDisabled, EventSeverity.Warning },
        { EventType.LoginFailed_SystemError, EventSeverity.Error },

        { EventType.PasswordChanged, EventSeverity.Informational },
        { EventType.UserCreated, EventSeverity.Informational },
        { EventType.UserDeleted, EventSeverity.Warning },
        { EventType.UserDisabled, EventSeverity.Warning },
        { EventType.UserEnabled, EventSeverity.Warning },
    };

        public static EventSeverity GetSeverity(EventType eventType)
        {
            return _eventSeverityMap[eventType];
        }
    }
}
