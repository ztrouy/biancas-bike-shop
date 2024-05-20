import { useEffect, useState } from "react";
import { Button, Input, Table } from "reactstrap";
import { getIncompleteWorkOrders, updateWorkOrder, completeWorkOrder, deleteIncompleteWorkOrder } from "../../managers/workOrderManager.js";
import { Link } from "react-router-dom";
import { getUserProfiles } from "../../managers/userProfileManager.js";


export default function WorkOrderList({ loggedInUser }) {
  const [workOrders, setWorkOrders] = useState([]);
  const [mechanics, setMechanics] = useState([])

  useEffect(() => {
    getIncompleteWorkOrders().then(setWorkOrders)
    getUserProfiles().then(setMechanics)
  }, []);

  const assignMechanic = (workOrder, mechanicId) => {
    const clone = structuredClone(workOrder)
    clone.userProfileId = mechanicId || null

    updateWorkOrder(clone).then(() => {
        getIncompleteWorkOrders().then(setWorkOrders)
    })
}

const handleCompleteWorkOrder = (workOrderId) => {
    completeWorkOrder(workOrderId).then(() => {
        getIncompleteWorkOrders().then(setWorkOrders)
    })
}

const handleDeleteIncompleteWorkOrder = (workOrderId) => {
    deleteIncompleteWorkOrder(workOrderId).then(() => {
        getIncompleteWorkOrders().then(setWorkOrders)
    })
}

  return (
    <>
      <h2>Open Work Orders</h2>
      <Link to={"/workorders/create"}>New Work Order</Link>
      <Table>
        <thead>
          <tr>
            <th>Owner</th>
            <th>Brand</th>
            <th>Color</th>
            <th>Description</th>
            <th>DateSubmitted</th>
            <th>Mechanic</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {workOrders.map((wo) => (
            <tr key={wo.id}>
              <th scope="row">{wo.bike.owner.name}</th>
              <td>{wo.bike.brand}</td>
              <td>{wo.bike.color}</td>
              <td>{wo.description}</td>
              <td>{new Date(wo.dateInitiated).toLocaleDateString()}</td>
              <td>
                <Input
                    type="select"
                    value={wo.userProfileId || 0}
                    onChange={event => assignMechanic(wo, parseInt(event.target.value))}
                >
                    <option value={0} key={0}>Choose mechanic</option>
                    {mechanics.map(m => (
                        <option value={m.id} key={m.id}>{`${m.firstName} ${m.lastName}`}</option>
                    ))}
                </Input>
              </td>
              <td>
                {wo.userProfile ? (
                    <Button 
                        onClick={() => handleCompleteWorkOrder(wo.id)}
                    >
                        Mark as Complete
                    </Button>
                ) : (
                    <Button
                        onClick={() => handleDeleteIncompleteWorkOrder(wo.id)}
                    >
                        Delete
                    </Button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  );
}