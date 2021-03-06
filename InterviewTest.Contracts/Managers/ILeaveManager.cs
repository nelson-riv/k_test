﻿using InterviewTest.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterviewTest.Contracts.Managers
{
    public interface ILeaveManager
    {
        bool NewLeave(Leave leave, out string message);

        Leave GetLeave(int leaveId);

        IEnumerable<Leave> GetAllLeaves();
        Leave UpdateLeave(Leave leave, out string message);
        bool DeleteLeave(int leaveId, out string message);
    }
}
