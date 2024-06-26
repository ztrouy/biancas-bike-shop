const _apiUrl = "/api/workorder"

export const getIncompleteWorkOrders = () => {
    return fetch(`${_apiUrl}/incomplete`).then(res => res.json())
}

export const createWorkOrder = (workOrder) => {
    const postOptions = {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(workOrder)
    }

    return fetch(_apiUrl, postOptions).then(res => res.json())
}

export const updateWorkOrder = (workOrder) => {
    const putOptions = {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(workOrder)
    }

    return fetch(`${_apiUrl}/${workOrder.id}`, putOptions)
}

export const completeWorkOrder = (id) => {
    const putOptions = {method: "PUT"}

    return fetch(`${_apiUrl}/${id}/complete`, putOptions)
}

export const deleteIncompleteWorkOrder = (id) => {
    const deleteOptions = {method: "DELETE"}

    return fetch(`${_apiUrl}/${id}`, deleteOptions)
}