import {
  Heading,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  IconButton,
  Badge,
} from '@chakra-ui/react';
import { FaTrashAlt, FaChevronDown, FaChevronUp } from 'react-icons/fa';
import { useState } from 'react';

import useAuth from '../../auth/useAuth';
import { useNavigate } from 'react-router-dom';

import formatCurrency from '../../utils/formatCurrency';
import formatDate from '../../utils/formatDate';

const PurchaseOrder = ({ purchaseOrder, onDelete, onExpand }) => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [expanded, setExpanded] = useState(false);

  const handleRowClick = () => {
    setExpanded(!expanded);
    onExpand(purchaseOrder, !expanded);
  };

  const handleDeleteClick = () => {
    onDelete(purchaseOrder);
  };

  return (
    <>
      <Tr
        key={purchaseOrder.id}
        cursor='pointer'
        _hover={{ bg: 'teal.50' }}
        onClick={handleRowClick}
        transition='background-color 0.3s ease, transform 0.3s ease'
        _active={{ transform: 'scale(0.99)' }}
      >
        <Td>
          <IconButton
            aria-label='Expand'
            icon={expanded ? <FaChevronUp /> : <FaChevronDown />}
            variant='ghost'
            size='sm'
            mr={2}
          />
          <Badge colorScheme='teal' mr={2}>
            {purchaseOrder.id}
          </Badge>
        </Td>
        <Td>{formatDate(purchaseOrder.createdAt)}</Td>
        <Td>{purchaseOrder.supplier.name}</Td>
        <Td isNumeric>{formatCurrency(purchaseOrder.total)}</Td>
        <Td>
          {user && user.role === 'admin' && (
            <IconButton
              aria-label='Delete'
              icon={<FaTrashAlt />}
              colorScheme='red'
              variant='ghost'
              size='sm'
              onClick={(e) => {
                e.stopPropagation();
                handleDeleteClick();
              }}
            />
          )}
        </Td>
      </Tr>
      {expanded && (
        <Tr>
          <Td colSpan={5} p={4}>
            <Heading size='sm' mb={2} color='teal.600'>
              Рядки замовлення
            </Heading>
            <Table variant='simple' size='sm'>
              <Thead>
                <Tr>
                  <Th>ID</Th>
                  <Th>Кількість</Th>
                </Tr>
              </Thead>
              <Tbody>
                {purchaseOrder.purchaseOrderLines.map((line) => (
                  <Tr
                    key={line.productId}
                    _hover={{ bg: 'teal.50', cursor: 'pointer' }}
                    transition='background-color 0.3s ease'
                    onClick={() => navigate(`/products/${line.productId}`)}
                  >
                    <Td>{line.productId}</Td>
                    <Td>{line.quantity}</Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Td>
        </Tr>
      )}
    </>
  );
};

export default PurchaseOrder;
